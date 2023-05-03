using System;
using System.Threading;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using MQTTnet;
using MQTTnet.Client;

namespace EngineIO.Samples
{
    class Program
    {
        private static IMqttClient _client;

        public static ProducerConfig config = new ProducerConfig()
        {
            BootstrapServers = "localhost:9092"
        };

        public static IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build();

        private static readonly HttpClient client = new HttpClient();

        public class ScaleValuesArray
        {
            private int[] scaleValues;

            public ScaleValuesArray()
            {
                scaleValues = new int[0];
            }

            public void AddScaleValue(int value)
            {
                int[] newArray = new int[scaleValues.Length + 1];
                for (int i = 0; i < scaleValues.Length; i++)
                {
                    newArray[i] = scaleValues[i];
                }
                newArray[scaleValues.Length] = value;
                scaleValues = newArray;
            }

            public void ClearScaleValues()
            {
                scaleValues = new int[0];
            }

            public int GetHighestScaleValue()
            {
                if (scaleValues.Length == 0)
                {
                    throw new InvalidOperationException("ScaleValuesArray is empty");
                }
                int highestValue = scaleValues[0];
                for (int i = 1; i < scaleValues.Length; i++)
                {
                    if (scaleValues[i] > highestValue)
                    {
                        highestValue = scaleValues[i];
                    }
                }
                return highestValue;
            }
        }

        public class Order
        {
            [JsonProperty("id")]
            public string Id { get; set; }
            [JsonProperty("instance")]
            public string Instance { get; set; }
            [JsonProperty("callback")]
            public string Callback { get; set; }
            [JsonProperty("palletCallback")]
            public string PalletCallback { get; set; }
            [JsonProperty("currentTask")]
            public string CurrentTask { get; set; }
            [JsonProperty("currentPalletTask")]
            public string CurrentPalletTask { get; set; }
            [JsonProperty("product")]
            public string Product { get; set; }
            [JsonProperty("tableStyle")]
            public string TableStyle { get; set; }
            [JsonProperty("mirrorShape")]
            public string MirrorShape { get; set; }
            [JsonProperty("doorType")]
            public string DoorType { get; set; }
            [JsonProperty("express")]
            public bool Express { get; set; }
            [JsonProperty("additionalEquipment")]
            public string AdditionalEquipment { get; set; }
            [JsonProperty("status")]
            public string Status { get; set; }
            [JsonProperty("palletStatus")]
            public string PalletStatus { get; set; }
            [JsonProperty("box")]
            public int Box { get; set; }
            [JsonProperty("logisticOption")]
            public int LogisticOption { get; set; }
            [JsonProperty("qualityAcceptable")]
            public bool QualityAcceptable { get; set; }
            [JsonProperty("direction")]
            public int Direction { get; set; }
            [JsonProperty("height")]
            public int Height { get; set; }
            [JsonProperty("weight")]
            public int Weight { get; set; }
        }

        public class Result
        {
            public string direction { get; set; }
        };

        public class Reference
        {
            public string reference { get; set; }
        }

        public class FactoryEvent
        {
            public string name { get; set; }
            public bool value { get; set; }
            public string type { get; set; }
            public long address { get; set; }
            public DateTime timestamp { get; set; }
        }

        static ScaleValuesArray scaleValuesArray = new ScaleValuesArray();

        // static void Main(string[] args)
        static async Task Main(string[] args)
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883) // Specify the broker address and port
                .Build();

            await _client.ConnectAsync(options);
            await client.GetAsync("http://host.docker.internal:9033/start/startContainer");
            Console.WriteLine("Producer gestartet.");
            MemoryMap.Instance.InputsValueChanged += new MemoriesChangedEventHandler(Instance_ValueChanged);
            MemoryMap.Instance.OutputsValueChanged += new MemoriesChangedEventHandler(Instance_ValueChanged);

            Console.WriteLine("Press any key to exit...");

            //Calling the Update method will fire events if any memory value or name changed.
            //When a Tag is created in Factory I/O a name is given to its memory, firing the name changed event, and when a tag's value is changed, it is fired the value changed event.
            //In this case we are updating the MemoryMap each 16 milliseconds (the typical update rate of Factory I/O).
            while (!Console.KeyAvailable)
            {
                MemoryMap.Instance.Update();

                Thread.Sleep(16);
            }
            await client.GetAsync("http://host.docker.internal:9033/start/endContainer");
            MemoryMap.Instance.Dispose();
        }

        static void createFObj(String name, String val, String type, Int64 address)
        {
            var floatVal = float.Parse(val);
            var intVal = (int)(floatVal * 100);
            if (name == "scale_weight" && floatVal > 0)
            {
                scaleValuesArray.AddScaleValue(intVal);
            }
        }

        async static void createIObj(String name, String val, String type, Int64 address)
        {
            var intVal = int.Parse(val);
            if (name == "height_light_array_emitter" && intVal > 0)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("measurement_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("measurement_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("measurement_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("measurement_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("measurement_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("measurement_rfid_execute", MemoryType.Output);
                var boxRef = await client.GetAsync("http://host.docker.internal:9033/start/preparePaket");
                var boxRefString = await boxRef.Content.ReadAsStringAsync();
                Reference reference = JsonConvert.DeserializeObject<Reference>(boxRefString);
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var order = await client.PostAsync("http://host.docker.internal:9033/start/details", new StringContent(body, Encoding.UTF8, "application/json"));
                var orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                // 1. Auf Index 1 Höhe hinterlegen
                memory_index.Value = 1;
                var amount = command_id.Value;
                writeRfid.Value = intVal;
                command.Value = 3;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                // 2. Referenznummer auf Index 0 hinterlegen
                memory_index.Value = 0;
                amount = command_id.Value;
                writeRfid.Value = int.Parse(reference.reference);
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                var msg = new JObject();
                msg["reference"] = reference.reference;
                msg["height"] = intVal;
                string payload = JsonConvert.SerializeObject(msg);
                await client.PostAsync("http://host.docker.internal:9033/start/determineHeight", new StringContent(payload, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
        }

        async static void createObj(String name, String val, String type, Int64 address)
        {
            bool isVal;
            if (val == "True")
            {
                isVal = true;
            }
            else
            {
                isVal = false;
            }
            if (name == "start_sensor" && isVal)
            {
                // Produkt wurde platziert
                var response = await client.GetAsync("http://host.docker.internal:9033/start/finished");
            }
            else if (name == "start_rfid_sensor" && isVal)
            {
                Console.WriteLine("Neues Produkt in der Fertigungslinie");
                var deviceId = "startsensor";
                var mqttMsg = new JObject();
                mqttMsg["val"] = 1;
                mqttMsg["timestamp"] = DateTime.Now;
                string payload = JsonConvert.SerializeObject(mqttMsg);
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic($"device/{deviceId}")
                    .WithPayload(payload)
                    .Build();
                await _client.PublishAsync(mqttMessage);
                MemoryInt command = MemoryMap.Instance.GetInt("start_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("start_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("start_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("start_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("start_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("start_rfid_execute", MemoryType.Output);
                command.Value = 1;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                // Referenznummer auf Index 0 hinterlegen
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                memory_index.Value = 0;
                amount = command_id.Value;
                writeRfid.Value = readRfid.Value;
                command.Value = 3;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_init", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var response = await client.PostAsync("http://host.docker.internal:9033/start/setup", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "direction_rfid_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("direction_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("direction_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("direction_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("direction_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("direction_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("direction_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var order = await client.PostAsync("http://host.docker.internal:9033/start/details", new StringContent(body, Encoding.UTF8, "application/json"));
                // var order = await client.GetAsync("http://host.docker.internal:9033/start/details");
                var orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                // Richtung auf Index 1 hinterlegen
                memory_index.Value = 1;
                command.Value = 3;
                amount = command_id.Value;
                writeRfid.Value = theOrder.Direction;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                memory_index.Value = 1;
                command.Value = 2;
                amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Console.WriteLine("Direction: " + readRfid.Value.ToString());
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "direction_end_sensor_left" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("before_machining_b_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("before_machining_b_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("before_machining_b_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("before_machining_b_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("before_machining_b_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("before_machining_b_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/saveSchrank", new StringContent(body, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "direction_forward_rfid_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("before_machining_a_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("before_machining_a_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("before_machining_a_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("before_machining_a_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("before_machining_a_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("before_machining_a_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/saveSchreibtisch", new StringContent(body, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "spawn_pallett_trigger_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("spawn_pallett_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("spawn_pallett_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("spawn_pallett_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("spawn_pallett_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("spawn_pallett_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("spawn_pallett_rfid_execute", MemoryType.Output);
                var palletRef = await client.GetAsync("http://host.docker.internal:9033/start/prepareSchrank");
                var palletRefString = await palletRef.Content.ReadAsStringAsync();
                Reference reference = JsonConvert.DeserializeObject<Reference>(palletRefString);
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var order = await client.PostAsync("http://host.docker.internal:9033/start/details", new StringContent(body, Encoding.UTF8, "application/json"));
                var orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                // 1. Referenznummer auf Index 0 hinterlegen
                memory_index.Value = 0;
                var amount = command_id.Value;
                writeRfid.Value = int.Parse(reference.reference);
                command.Value = 3;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Console.WriteLine("Palette mit Referenz verbunden: " + writeRfid.Value.ToString());
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                // Auf Index 1 Türtyp hinterlegen
                memory_index.Value = 1;
                amount = command_id.Value;
                if (theOrder.DoorType != null)
                {
                    if (theOrder.DoorType == "default")
                    {
                        writeRfid.Value = 0;
                    }
                    else
                    {
                        writeRfid.Value = 1;
                    }
                    rfid.Value = true;
                    while (command_id.Value <= amount)
                    {
                        MemoryMap.Instance.Update();
                        Thread.Sleep(16);
                    }
                    Console.WriteLine("DoorType: " + writeRfid.Value.ToString());
                    rfid.Value = false;
                    MemoryMap.Instance.Update();
                }
            }
            else if (name == "after_machining_a_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("after_machining_a_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("after_machining_a_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("after_machining_a_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("after_machining_a_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("after_machining_a_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("after_machining_a_rfid_execute", MemoryType.Output);
                var palletRef = await client.GetAsync("http://host.docker.internal:9033/start/prepareSchreibtisch");
                var palletRefString = await palletRef.Content.ReadAsStringAsync();
                Reference reference = JsonConvert.DeserializeObject<Reference>(palletRefString);
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var order = await client.PostAsync("http://host.docker.internal:9033/start/details", new StringContent(body, Encoding.UTF8, "application/json"));
                var orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                // 1. Referenznummer auf Index 0 hinterlegen
                memory_index.Value = 0;
                var amount = command_id.Value;
                writeRfid.Value = int.Parse(reference.reference);
                command.Value = 3;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Console.WriteLine("Palette mit Referenz verbunden: " + writeRfid.Value.ToString());
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "pre_drill_a_sensor" && isVal)
            {
                Console.WriteLine("Pre-Drill.");
                MemoryInt command = MemoryMap.Instance.GetInt("pre_drill_a_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("pre_drill_a_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("pre_drill_a_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("pre_drill_a_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("pre_drill_a_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("pre_drill_a_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_pre_drill", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "drawers_sensor" && isVal)
            {
                Console.WriteLine("Drawers.");
                MemoryInt command = MemoryMap.Instance.GetInt("drawers_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("drawers_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("drawers_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("drawers_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("drawers_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("drawers_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_drawers_production", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "after_machining_b_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("after_machining_b_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("after_machining_b_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("after_machining_b_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("after_machining_b_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("after_machining_b_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("after_machining_b_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "elevator_1_right_limit" && !isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("elevator_1_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("elevator_1_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("elevator_1_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("elevator_1_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("elevator_1_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("elevator_1_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/mirrorReference", new StringContent(body, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "second_floor_trigger_sensor" && isVal) {
                Console.WriteLine("Prepare Mirror Material.");
                MemoryInt command = MemoryMap.Instance.GetInt("second_floor_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("second_floor_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("second_floor_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("second_floor_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("second_floor_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("second_floor_rfid_execute", MemoryType.Output);
                var mirrorRef = await client.GetAsync("http://host.docker.internal:9033/start/prepareSpiegel");
                var mirrorRefString = await mirrorRef.Content.ReadAsStringAsync();
                Reference reference = JsonConvert.DeserializeObject<Reference>(mirrorRefString);
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                // 1. Referenznummer auf Index 0 hinterlegen
                memory_index.Value = 0;
                var amount = command_id.Value;
                writeRfid.Value = int.Parse(reference.reference);
                command.Value = 3;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                await client.PostAsync("http://host.docker.internal:9033/start/completeMirrorTasks", new StringContent(body, Encoding.UTF8, "application/json"));
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_prepare_mirror_material", MemoryType.Output);
                simulateLight.Value = false;
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "pack_rfid_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("pack_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("pack_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("pack_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("pack_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("pack_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("pack_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/boxReference", new StringContent(body, Encoding.UTF8, "application/json"));
                // box size hinterlegen auf index 1
                var order = await client.PostAsync("http://host.docker.internal:9033/start/details", new StringContent(body, Encoding.UTF8, "application/json"));
                var orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                memory_index.Value = 1;
                command.Value = 3;
                writeRfid.Value = theOrder.Box;
                amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "scale_forward" && isVal)
            {
                scaleValuesArray = new ScaleValuesArray();
            }
            else if (name == "after_pack_sensor" && isVal)
            {
                int highestValue = scaleValuesArray.GetHighestScaleValue();
                MemoryInt command = MemoryMap.Instance.GetInt("measurement_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("measurement_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("measurement_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("measurement_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("measurement_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("measurement_rfid_execute", MemoryType.Output);
                // Gewicht auf Index 2 hinterlegen
                memory_index.Value = 2;
                var amount = command_id.Value;
                writeRfid.Value = highestValue;
                command.Value = 3;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                memory_index.Value = 0;
                command.Value = 2;
                amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                var msg = new JObject();
                msg["reference"] = reference.reference;
                msg["weight"] = highestValue;
                string payload = JsonConvert.SerializeObject(msg);
                await client.PostAsync("http://host.docker.internal:9033/start/determineWeight", new StringContent(payload, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "table_style_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("table_style_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("table_style_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("table_style_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("table_style_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("table_style_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("table_style_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryBit greenLight = MemoryMap.Instance.GetBit("classic", MemoryType.Output);
                MemoryBit yellowLight = MemoryMap.Instance.GetBit("modern", MemoryType.Output);
                greenLight.Value = false;
                yellowLight.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "table_legs_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("table_legs_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("table_legs_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("table_legs_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("table_legs_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("table_legs_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("table_legs_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_table_legs_production", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
            }
            else if ((name == "before_check_direction_sensor" && isVal) || (name == "check_rfid_sensor" && isVal))
            {
                Console.WriteLine("Determine Quality.");
                MemoryInt command = MemoryMap.Instance.GetInt("check_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("check_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("check_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("check_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("check_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("check_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                // Resultat abfragen und auf RFID Index 1 schreiben
                var order = await client.PostAsync("http://host.docker.internal:9033/start/details", new StringContent(body, Encoding.UTF8, "application/json"));
                var orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                memory_index.Value = 1;
                command.Value = 3;
                if (theOrder.QualityAcceptable == true) {
                    writeRfid.Value = 0;
                } else {
                    writeRfid.Value = 1;
                }
                amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                // RFID lesen
                memory_index.Value = 1;
                command.Value = 2;
                amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Console.WriteLine("Direction (2): " + readRfid.Value.ToString());
                rfid.Value = false;
                MemoryMap.Instance.Update();
                var jsonBody = new JObject();
                jsonBody["reference"] = reference.reference;
                jsonBody["quality"] = theOrder.QualityAcceptable;
                string payload = JsonConvert.SerializeObject(jsonBody);
                await client.PostAsync("http://host.docker.internal:9033/start/completeQuality", new StringContent(payload, Encoding.UTF8, "application/json"));
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_quality_check", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "machining_A_busy" && !isVal)
            {
                MemoryBit machine = MemoryMap.Instance.GetBit("machining_A_start", MemoryType.Output);
                machine.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "machining_B_busy" && !isVal)
            {
                MemoryBit machine = MemoryMap.Instance.GetBit("machining_B_start", MemoryType.Output);
                machine.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "varnishing_rfid_sensor" && isVal)
            {
                // Referenznummer lesen
                MemoryInt command = MemoryMap.Instance.GetInt("varnishing_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("varnishing_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("varnishing_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("varnishing_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("varnishing_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("varnishing_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "varnishing_end_sensor" && isVal)
            {
                // Referenznummer für Request
                MemoryInt readRfid = MemoryMap.Instance.GetInt("varnishing_rfid_read", MemoryType.Input);
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "end_sensor" && isVal)
            {
                var deviceId = "endsensor";
                var mqttMsg = new JObject();
                mqttMsg["val"] = 1;
                mqttMsg["timestamp"] = DateTime.Now;
                string payload = JsonConvert.SerializeObject(mqttMsg);
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic($"device/{deviceId}")
                    .WithPayload(payload)
                    .Build();
                await _client.PublishAsync(mqttMessage);
            }
            else if (name == "varnishing_stopper_1_2")
            {
                MemoryFloat tank_levelMeter = MemoryMap.Instance.GetFloat("tank_levelMeter", MemoryType.Input);
                var deviceId = "tank";
                var mqttMsg = new JObject();
                mqttMsg["val"] = (float)Math.Round(tank_levelMeter.Value, 2);
                mqttMsg["timestamp"] = DateTime.Now;
                string payload = JsonConvert.SerializeObject(mqttMsg);
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic($"device/{deviceId}")
                    .WithPayload(payload)
                    .Build();
                await _client.PublishAsync(mqttMessage);
            }
            else if (name == "pre_drill_b_sensor" && isVal)
            {
                Console.WriteLine("Pre-Drill.");
                MemoryInt command = MemoryMap.Instance.GetInt("pre_drill_b_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("pre_drill_b_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("pre_drill_b_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("pre_drill_b_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("pre_drill_b_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("pre_drill_b_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_pre_drill_schrank", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "spawn_pallett_end_sensor" && isVal)
            {
                Console.WriteLine("Shelves.");
                MemoryInt command = MemoryMap.Instance.GetInt("shelves_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("shelves_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("shelves_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("shelves_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("shelves_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("shelves_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_shelves_production", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "door_type_sensor" && isVal)
            {
                Console.WriteLine("Door Type.");
                MemoryInt command = MemoryMap.Instance.GetInt("door_type_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("door_type_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("door_type_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("door_type_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("door_type_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("door_type_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit sliding = MemoryMap.Instance.GetBit("slidingDoor", MemoryType.Output);
                MemoryBit defaultDoor = MemoryMap.Instance.GetBit("defaultDoor", MemoryType.Output);
                sliding.Value = false;
                defaultDoor.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "lock_sensor" && isVal)
            {
                Console.WriteLine("Lock.");
                MemoryInt command = MemoryMap.Instance.GetInt("lock_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("lock_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("lock_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("lock_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("lock_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("lock_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_lock_production", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "elevator_2_end_sensor" && isVal)
            {
                Console.WriteLine("Assemble.");
                MemoryInt command = MemoryMap.Instance.GetInt("assemble_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("assemble_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("assemble_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("assemble_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("assemble_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("assemble_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_assemble", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "extra_stuff_sensor" && isVal)
            {
                Console.WriteLine("Extra Stuff.");
                MemoryInt command = MemoryMap.Instance.GetInt("extra_stuff_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("extra_stuff_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("extra_stuff_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("extra_stuff_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("extra_stuff_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("extra_stuff_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_extra_parts", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "improve_sensor" && isVal)
            {
                Console.WriteLine("Improve.");
                MemoryInt command = MemoryMap.Instance.GetInt("improve_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("improve_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("improve_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("improve_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("improve_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("improve_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_improve_quality", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "additional_equipment_sensor" && isVal)
            {
                Console.WriteLine("Additional Equipment.");
                MemoryInt command = MemoryMap.Instance.GetInt("additional_equipment_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("additional_equipment_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("additional_equipment_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("additional_equipment_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("additional_equipment_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("additional_equipment_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_additional_equipment", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "shape_sensor" && isVal)
            {
                Console.WriteLine("Mirror Shape.");
                MemoryInt command = MemoryMap.Instance.GetInt("shape_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("shape_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("shape_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("shape_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("shape_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("shape_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/completeMirrorTasks", new StringContent(body, Encoding.UTF8, "application/json"));
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                MemoryBit angular = MemoryMap.Instance.GetBit("angular", MemoryType.Output);
                MemoryBit circular = MemoryMap.Instance.GetBit("circular", MemoryType.Output);
                angular.Value = false;
                circular.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "pack_trigger_sensor" && isVal)
            {
                Console.WriteLine("Box Size");
                MemoryInt readRfid = MemoryMap.Instance.GetInt("pack_rfid_read", MemoryType.Input);
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                // rfid leser wechseln
                MemoryInt command = MemoryMap.Instance.GetInt("box_rfid_command", MemoryType.Output);
                readRfid = MemoryMap.Instance.GetInt("box_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("box_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("box_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("box_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("box_rfid_execute", MemoryType.Output);
                memory_index.Value = 1;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                MemoryBit remover = MemoryMap.Instance.GetBit("pack_remover", MemoryType.Output);
                MemoryBit emitter = MemoryMap.Instance.GetBit("pack_emitter", MemoryType.Output);
                remover.Value = true;
                MemoryMap.Instance.Update();
                await Task.Delay(200);
                remover.Value = false;
                emitter.Value = true;
                MemoryMap.Instance.Update();
                await Task.Delay(200);
                emitter.Value = false;
                MemoryMap.Instance.Update();
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "pusher_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("measurement_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("measurement_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("measurement_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("measurement_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("measurement_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("measurement_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                var amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var order = await client.PostAsync("http://host.docker.internal:9033/start/details", new StringContent(body, Encoding.UTF8, "application/json"));
                var orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                //xx schreiben index 3
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                memory_index.Value = 3;
                command.Value = 3;
                writeRfid.Value = theOrder.LogisticOption;
                amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                //xx lesen index 3
                memory_index.Value = 3;
                command.Value = 2;
                amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "logistic_option_1_task_sensor" && isVal)
            {
                Console.WriteLine("Pusher Option 1");
                MemoryInt command = MemoryMap.Instance.GetInt("option_1_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("option_1_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("option_1_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("option_1_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("option_1_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("option_1_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "logistic_option_2_task_sensor" && isVal)
            {
                Console.WriteLine("Pusher Option 2");
                MemoryInt command = MemoryMap.Instance.GetInt("option_2_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("option_2_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("option_2_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("option_2_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("option_2_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("option_2_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
            else if (name == "logistic_option_3_task_sensor" && isVal)
            {
                Console.WriteLine("Pusher Option 3");
                MemoryInt command = MemoryMap.Instance.GetInt("option_3_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("option_3_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("option_3_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("option_3_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("option_3_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("option_3_rfid_execute", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
            }
        }

        static void Instance_ValueChanged(MemoryMap sender, MemoriesChangedEventArgs value)
        {
            //Display any changed MemoryBit
            foreach (MemoryBit mem in value.MemoriesBit)
            {
                createObj(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
            }

            // Display any changed MemoryFLoat
            foreach (MemoryFloat mem in value.MemoriesFloat)
            {
                createFObj(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
            }

            //Display any changed MemoryInt
            foreach (MemoryInt mem in value.MemoriesInt)
            {
                createIObj(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
            }
        }
    }
}