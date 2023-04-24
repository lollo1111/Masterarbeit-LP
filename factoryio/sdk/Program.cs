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
            [JsonProperty("currentTask")]
            public string CurrentTask { get; set; }
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
            [JsonProperty("box")]
            public int Box { get; set; }
            [JsonProperty("logisticOption")]
            public int LogisticOption { get; set; }
            [JsonProperty("qualityAcceptable")]
            public bool QualityAcceptable { get; set; }
            [JsonProperty("direction")]
            public int Direction { get; set; }
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
                Console.WriteLine("Strange: " + boxRefString);
                Reference reference = JsonConvert.DeserializeObject<Reference>(boxRefString);
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
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                // 2. Auf Index 1 Höhe hinterlegen
                memory_index.Value = 1;
                amount = command_id.Value;
                writeRfid.Value = intVal;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
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
                var order = await client.GetAsync("http://host.docker.internal:9033/start/details");
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
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                // Spiegelform auf Index 1 hinterlegen
                memory_index.Value = 1;
                amount = command_id.Value;
                if (theOrder.MirrorShape != null)
                {
                    if (theOrder.MirrorShape == "eckig")
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
                    rfid.Value = false;
                    MemoryMap.Instance.Update();
                }
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
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync("http://host.docker.internal:9033/start/boxReference", new StringContent(body, Encoding.UTF8, "application/json"));
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
            else if ((name == "before_check_direction_sensor" || name == "check_rfid_sensor") && isVal)
            {
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
                await client.PostAsync("http://host.docker.internal:9033/start/complete", new StringContent(body, Encoding.UTF8, "application/json"));
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