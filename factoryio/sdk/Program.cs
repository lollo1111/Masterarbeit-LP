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

        private static readonly HttpClient client = new HttpClient();

        public static async Task addReference(int rfidRef, string endpoint)
        {
            try
            {
                Reference reference = new Reference
                {
                    reference = rfidRef.ToString(),
                };
                string payload = System.Text.Json.JsonSerializer.Serialize(reference);
                await client.PostAsync(endpoint, new StringContent(payload, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }
        public static async Task completeTask(int rfidRef, string endpoint = "http://host.docker.internal:9033/tasks/complete", string payload = "")
        {
            try
            {
                Reference reference = new Reference
                {
                    reference = rfidRef.ToString(),
                };
                if (payload == "")
                {
                    payload = System.Text.Json.JsonSerializer.Serialize(reference);
                }
                // Console.WriteLine("Endpoint: " + endpoint + ", body: " + payload);
                var request = new HttpRequestMessage(HttpMethod.Patch, endpoint);
                request.Content = new StringContent(payload, Encoding.UTF8, "application/json");
                await client.SendAsync(request);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn error occurred: {ex.Message}");
            }
        }

        public static async Task sendTopic(string deviceId, object deviceValue)
        {
            var mqttMsg = new JObject();
            if (deviceValue is string)
            {
                mqttMsg["val"] = deviceValue as string;
            }
            else if (deviceValue is int)
            {
                mqttMsg["val"] = (int)deviceValue;
            }
            else if (deviceValue is bool)
            {
                mqttMsg["val"] = (bool)deviceValue;
            }
            else if (deviceValue is float)
            {
                mqttMsg["val"] = (float)deviceValue;
            }
            mqttMsg["timestamp"] = DateTime.Now;
            string payload = JsonConvert.SerializeObject(mqttMsg);
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic($"device/{deviceId}")
                .WithPayload(payload)
                .Build();
            await _client.PublishAsync(mqttMessage);
        }

        public class RfidDevice
        {
            private int amount;
            private MemoryInt command, readRfid, writeRfid, memory_index, command_id, status;
            private MemoryBit rfid;

            public RfidDevice(string rfidName)
            {
                command = MemoryMap.Instance.GetInt((rfidName + "_command"), MemoryType.Output);
                readRfid = MemoryMap.Instance.GetInt((rfidName + "_read"), MemoryType.Input);
                writeRfid = MemoryMap.Instance.GetInt((rfidName + "_write"), MemoryType.Output);
                memory_index = MemoryMap.Instance.GetInt((rfidName + "_memoryindex"), MemoryType.Output);
                command_id = MemoryMap.Instance.GetInt((rfidName + "_id"), MemoryType.Input);
                status = MemoryMap.Instance.GetInt((rfidName + "_status"), MemoryType.Input);
                rfid = MemoryMap.Instance.GetBit((rfidName + "_execute"), MemoryType.Output);
            }

            public async Task<bool> foundTag(int delay)
            {
                // prüfen ob rfid tag erkannt
                int counter = 0;
                command.Value = 0;
                amount = command_id.Value;
                do
                {
                    rfid.Value = true;
                    while (command_id.Value <= amount)
                    {
                        MemoryMap.Instance.Update();
                        await Task.Delay(16);
                    }
                    ++counter;
                    // Console.WriteLine("STATUS (counter: " + counter + "): " + status.Value.ToString());
                    rfid.Value = false;
                    MemoryMap.Instance.Update();
                    await Task.Delay(delay);
                    if (counter > 2)
                    {
                        break;
                    }
                } while (status.Value != 0);
                if (status.Value == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public async Task<int> readReference(int delay = 100, bool checkTag = true)
            {
                try
                {
                    bool found = true;
                    if (checkTag) {
                        found = await foundTag(delay);
                    }
                    if (found)
                    {
                        amount = command_id.Value;
                        command.Value = 1;
                        rfid.Value = true;
                        while (command_id.Value <= amount)
                        {
                            MemoryMap.Instance.Update();
                            await Task.Delay(16);
                        }
                        rfid.Value = false;
                        MemoryMap.Instance.Update();
                        await Task.Delay(delay);
                        if (status.Value != 0)
                        {
                            throw new Exception("Status not equal to 0");
                        }
                        return readRfid.Value;
                    }
                    else
                    {
                        throw new Exception("No Rfid Tag found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nAn error occurred: {ex.Message}");
                    return 0;
                }
            }

            public async Task<int> readValue(int index, bool checkTag = true, int delay = 100)
            {
                try
                {       
                    bool found = true;
                    if (checkTag) {
                        found = await foundTag(delay);
                    }
                    if (found)
                    {
                        memory_index.Value = index;
                        amount = command_id.Value;
                        command.Value = 2;
                        rfid.Value = true;
                        while (command_id.Value <= amount)
                        {
                            MemoryMap.Instance.Update();
                            await Task.Delay(16);
                        }
                        rfid.Value = false;
                        MemoryMap.Instance.Update();
                        await Task.Delay(delay);
                        if (status.Value != 0)
                        {
                            throw new Exception("Status not equal to 0");
                        }
                        return readRfid.Value;
                    }
                    else
                    {
                        throw new Exception("No Rfid Tag found.");
                    }         
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nAn error occurred: {ex.Message}");
                    return 0;
                }
            }

            public async Task writeValue(int index, int writeVal, int delay = 100)
            {
                try
                {
                    memory_index.Value = index;
                    amount = command_id.Value;
                    command.Value = 3;
                    writeRfid.Value = writeVal;
                    rfid.Value = true;
                    while (command_id.Value <= amount)
                    {
                        MemoryMap.Instance.Update();
                        await Task.Delay(16);
                    }
                    rfid.Value = false;
                    MemoryMap.Instance.Update();
                    await Task.Delay(delay);
                    if (status.Value != 0)
                    {
                        throw new Exception("Status not equal to 0");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nAn error occurred: {ex.Message}");
                }
            }
            public async Task<Order> getOrder(int rfidRef)
            {
                try
                {
                    Reference reference = new Reference
                    {
                        reference = rfidRef.ToString(),
                    };
                    string body = System.Text.Json.JsonSerializer.Serialize(reference);
                    var order = await client.PostAsync("http://host.docker.internal:9033/tasks/details", new StringContent(body, Encoding.UTF8, "application/json"));
                    string orderString = await order.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Order>(orderString);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nAn error occurred: {ex.Message}");
                    return null;
                }
            }
        }

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

            public int GetAverageScaleValue()
            {
                if (scaleValues.Length == 0)
                {
                    throw new InvalidOperationException("ScaleValuesArray is empty");
                }
                int sum = 0;
                for (int i = 0; i < scaleValues.Length; i++) {
                    sum += scaleValues[i];
                }
                int average = sum / scaleValues.Length;
                Console.WriteLine("Average Box Weight: " + average);
                return average;
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

        public class Reference
        {
            public string reference { get; set; }
        }

        static ScaleValuesArray scaleValuesArray = new ScaleValuesArray();

        static async Task Main(string[] args)
        {
            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("localhost", 1883) // Specify the broker address and port
                .Build();

            await _client.ConnectAsync(options);
            await client.GetAsync("http://host.docker.internal:9033/healthcheck/startContainer");
            Console.WriteLine("Producer gestartet.");
            MemoryMap.Instance.InputsValueChanged += new MemoriesChangedEventHandler(Instance_ValueChanged);
            MemoryMap.Instance.OutputsValueChanged += new MemoriesChangedEventHandler(Instance_ValueChanged);

            //Calling the Update method will fire events if any memory value or name changed.
            //When a Tag is created in Factory I/O a name is given to its memory, firing the name changed event, and when a tag's value is changed, it is fired the value changed event.
            //In this case we are updating the MemoryMap each 16 milliseconds (the typical update rate of Factory I/O).
            while (!Console.KeyAvailable)
            {
                MemoryMap.Instance.Update();
                await Task.Delay(16);
            }
            await client.GetAsync("http://host.docker.internal:9033/healthcheck/endContainer");
            MemoryMap.Instance.Dispose();
            Console.WriteLine("SDK wird beendet ...\n");
            Environment.Exit(0);
        }

        static void floatValueChange(String name, String val, String type, Int64 address)
        {
            float floatVal = float.Parse(val);
            int intVal = (int)(floatVal * 100);
            if (name == "scale_weight" && intVal > 50)
            {
                scaleValuesArray.AddScaleValue(intVal);
            }
        }

        async static void intValueChange(String name, String val, String type, Int64 address)
        {
            int intVal = int.Parse(val);
            if (name == "height_light_array_emitter" && intVal > 0)
            {
                MemoryBit remover = MemoryMap.Instance.GetBit("pack_remover", MemoryType.Output);
                RfidDevice rfidDevice = new RfidDevice("measurement_rfid");              
                var boxRef = await client.GetAsync("http://host.docker.internal:9033/tasks/preparePaket");
                string boxRefString = await boxRef.Content.ReadAsStringAsync();
                Reference reference = JsonConvert.DeserializeObject<Reference>(boxRefString);
                Order theOrder = await rfidDevice.getOrder(int.Parse(reference.reference));
                remover.Value = false;
                MemoryMap.Instance.Update();
                // 1. Auf Index 1 Höhe hinterlegen
                await rfidDevice.writeValue(1, intVal);
                // 2. Referenznummer auf Index 0 hinterlegen
                await rfidDevice.writeValue(0, int.Parse(reference.reference));
                var msg = new JObject();
                msg["reference"] = reference.reference;
                msg["height"] = intVal;
                string payload = JsonConvert.SerializeObject(msg);
                await completeTask(int.Parse(reference.reference), "http://host.docker.internal:9033/tasks/determineHeight", payload);
            }
        }

        async static void bitValueChange(String name, String val, String type, Int64 address)
        {
            bool isVal = bool.Parse(val);
            if (name == "start_sensor" && isVal)
            {
                Console.WriteLine("Neues Produkt in der Fertigungslinie");
                var request = new HttpRequestMessage(HttpMethod.Patch, "http://host.docker.internal:9033/tasks/placed");
                await client.SendAsync(request);
            }
            else if (name == "start_rfid_sensor" && isVal)
            {
                Console.WriteLine("Material vorbereitet.");
                await sendTopic("startsensor", 1);
                RfidDevice rfidDevice = new RfidDevice("start_rfid");
                int palletReference = await rfidDevice.readReference();
                await rfidDevice.writeValue(0, palletReference);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_init", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference, "http://host.docker.internal:9033/tasks/setup");
            }
            else if (name == "direction_rfid_sensor" && isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("direction_rfid");
                int palletReference = await rfidDevice.readValue(0);
                Order theOrder = await rfidDevice.getOrder(palletReference);
                // Richtung auf Index 1 hinterlegen
                await rfidDevice.writeValue(1, theOrder.Direction);
                await sendTopic("product", theOrder.Direction == 0 ? "A" : "B");
                // Lesen
                int direction = await rfidDevice.readValue(1, false);
                Console.WriteLine("Direction: " + direction.ToString());
            }
            else if (name == "direction_end_sensor_left" && isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("before_machining_b_rfid");
                int palletReference = await rfidDevice.readValue(0);
                await addReference(palletReference, "http://host.docker.internal:9033/tasks/saveSchrank");
            }
            else if (name == "direction_forward_rfid_sensor" && isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("before_machining_a_rfid");
                int palletReference = await rfidDevice.readValue(0);
                await addReference(palletReference, "http://host.docker.internal:9033/tasks/saveSchreibtisch");
            }
            else if (name == "spawn_pallett_trigger_sensor" && isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("spawn_pallett_rfid");
                var palletRef = await client.GetAsync("http://host.docker.internal:9033/tasks/prepareSchrank");
                string palletRefString = await palletRef.Content.ReadAsStringAsync();
                Reference reference = JsonConvert.DeserializeObject<Reference>(palletRefString);
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var order = await client.PostAsync("http://host.docker.internal:9033/tasks/details", new StringContent(body, Encoding.UTF8, "application/json"));
                string orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                // 1. Referenznummer auf Index 0 hinterlegen
                await rfidDevice.writeValue(0, int.Parse(reference.reference));
                Console.WriteLine("Palette mit Referenz verbunden: " + reference.reference);
                // Auf Index 1 Türtyp hinterlegen
                if (theOrder.DoorType != null)
                {
                    if (theOrder.DoorType == "default")
                    {
                        await rfidDevice.writeValue(1, 0);
                        Console.WriteLine("Default Door");
                    }
                    else
                    {
                        await rfidDevice.writeValue(1, 1);
                        Console.WriteLine("Sliding Door");
                    }
                }
            }
            else if (name == "after_machining_a_sensor" && isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("after_machining_a_rfid");              
                var palletRef = await client.GetAsync("http://host.docker.internal:9033/tasks/prepareSchreibtisch");
                string palletRefString = await palletRef.Content.ReadAsStringAsync();
                Reference reference = JsonConvert.DeserializeObject<Reference>(palletRefString);
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var order = await client.PostAsync("http://host.docker.internal:9033/tasks/details", new StringContent(body, Encoding.UTF8, "application/json"));
                string orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                // 1. Referenznummer auf Index 0 hinterlegen
                await rfidDevice.writeValue(0, int.Parse(reference.reference));
                Console.WriteLine("Palette mit Referenz verbunden: " + reference.reference);
                await completeTask(int.Parse(reference.reference));
            }
            else if (name == "pre_drill_a_sensor" && isVal)
            {
                Console.WriteLine("Vorbohren.");
                RfidDevice rfidDevice = new RfidDevice("pre_drill_a_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_pre_drill", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "drawers_sensor" && isVal)
            {
                Console.WriteLine("Drawers.");
                RfidDevice rfidDevice = new RfidDevice("drawers_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_drawers_production", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "after_machining_b_sensor" && isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("after_machining_b_rfid");
                int palletReference = await rfidDevice.readValue(0);
                await completeTask(palletReference);
            }
            else if (name == "elevator_1_right_limit" && !isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("elevator_1_rfid");
                int palletReference = await rfidDevice.readValue(0);
                await addReference(palletReference, "http://host.docker.internal:9033/tasks/mirrorReference");
            }
            else if (name == "second_floor_trigger_sensor" && isVal) {
                Console.WriteLine("Prepare Mirror Material.");
                RfidDevice rfidDevice = new RfidDevice("second_floor_rfid");
                var mirrorRef = await client.GetAsync("http://host.docker.internal:9033/tasks/prepareSpiegel");
                string mirrorRefString = await mirrorRef.Content.ReadAsStringAsync();
                Reference reference = JsonConvert.DeserializeObject<Reference>(mirrorRefString);
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                // 1. Referenznummer auf Index 0 hinterlegen
                await rfidDevice.writeValue(0, int.Parse(reference.reference));
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_prepare_mirror_material", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(int.Parse(reference.reference), "http://host.docker.internal:9033/tasks/completeMirrorTasks");
            }
            else if (name == "pack_rfid_sensor" && isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("pack_rfid");
                int palletReference = await rfidDevice.readValue(0);
                await addReference(palletReference, "http://host.docker.internal:9033/tasks/boxReference");
                Order theOrder = await rfidDevice.getOrder(palletReference);
                await rfidDevice.writeValue(1, theOrder.Box);
            }
            else if (name == "scale_forward" && isVal)
            {
                scaleValuesArray = new ScaleValuesArray();
            }
            else if (name == "after_pack_sensor" && isVal)
            {
                int averageValue = scaleValuesArray.GetAverageScaleValue();
                RfidDevice rfidDevice = new RfidDevice("measurement_rfid");
                await rfidDevice.writeValue(2, averageValue);
                int palletReference = await rfidDevice.readValue(0);
                var msg = new JObject();
                msg["reference"] = palletReference.ToString();
                msg["weight"] = averageValue;
                string payload = JsonConvert.SerializeObject(msg);
                await completeTask(palletReference, "http://host.docker.internal:9033/tasks/determineWeight", payload);
            }
            else if (name == "table_style_sensor" && isVal)
            {
                Console.WriteLine("Table Style.");
                RfidDevice rfidDevice = new RfidDevice("table_style_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit greenLight = MemoryMap.Instance.GetBit("classic", MemoryType.Output);
                MemoryBit yellowLight = MemoryMap.Instance.GetBit("modern", MemoryType.Output);
                greenLight.Value = false;
                yellowLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "table_legs_sensor" && isVal)
            {
                Console.WriteLine("Table Legs.");
                RfidDevice rfidDevice = new RfidDevice("table_legs_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_table_legs_production", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if ((name == "before_check_direction_sensor" && isVal) || (name == "check_rfid_sensor" && isVal))
            {
                Console.WriteLine("Determine Quality: " + name );
                if (name == "check_rfid_sensor")
                {
                    await Task.Delay(1500);
                }
                RfidDevice rfidDevice = new RfidDevice("check_rfid");
                int palletReference = await rfidDevice.readValue(0);             
                // Resultat abfragen und auf RFID Index 1 schreiben
                Order theOrder = await rfidDevice.getOrder(palletReference);
                await sendTopic("quality", theOrder.QualityAcceptable); 
                if (theOrder.QualityAcceptable == true) {
                    await rfidDevice.writeValue(1, 0);
                } else {
                    await rfidDevice.writeValue(1, 1);
                }
                // RFID lesen
                int quality = await rfidDevice.readValue(1, false);
                Console.WriteLine("Reference: " + palletReference.ToString() + ", Quality Direction: " + quality.ToString());
                var jsonBody = new JObject();
                jsonBody["reference"] = palletReference.ToString();
                jsonBody["quality"] = theOrder.QualityAcceptable;
                string payload = JsonConvert.SerializeObject(jsonBody);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_quality_check", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference, "http://host.docker.internal:9033/tasks/completeQuality", payload);
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
                RfidDevice rfidDevice = new RfidDevice("varnishing_rfid");
                await rfidDevice.readValue(0);  
            }
            else if (name == "varnishing_end_sensor" && isVal)
            {
                Console.WriteLine("Varnishing.");
                MemoryInt readRfid = MemoryMap.Instance.GetInt("varnishing_rfid_read", MemoryType.Input);
                await completeTask(readRfid.Value);
            }
            else if (name == "end_sensor" && isVal)
            {
                Console.WriteLine("Send.");
                MemoryBit remover = MemoryMap.Instance.GetBit("after_logistic_remover", MemoryType.Output);
                RfidDevice rfidDevice = new RfidDevice("product_shipment_rfid");
                int palletReference = await rfidDevice.readValue(0);
                // nach 50s wird counter um 1 gekürzt
                await completeTask(palletReference);
                remover.Value = true;
                MemoryMap.Instance.Update();
                await sendTopic("endsensor", 1);              
                await Task.Delay(100);
                remover.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "end_sensor_2" && isVal)
            {
                MemoryInt readRfidZero = MemoryMap.Instance.GetInt("palletizer_rfid_read", MemoryType.Input);
                MemoryInt readRfidOne = MemoryMap.Instance.GetInt("pallet_shipment_rfid_read", MemoryType.Input);
                int firstBox = readRfidZero.Value;
                int secondBox = readRfidOne.Value;
                MemoryBit remover = MemoryMap.Instance.GetBit("after_palletizer_remover", MemoryType.Output);             
                // 3x, für jedes item
                remover.Value = true;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                remover.Value = false;
                remover.Value = true;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                remover.Value = false;
                remover.Value = true;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                remover.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(firstBox);
                await completeTask(secondBox);
                await sendTopic("endsensor", 1);
                await sendTopic("endsensor", 1); 
            }
            else if (name == "varnishing_stopper_1_2")
            {
                MemoryFloat tank_levelMeter = MemoryMap.Instance.GetFloat("tank_levelMeter", MemoryType.Input);
                await sendTopic("tank", ((float)Math.Round(tank_levelMeter.Value, 2)));
            }
            else if (name == "pre_drill_b_sensor" && isVal)
            {
                Console.WriteLine("Pre-Drill.");
                RfidDevice rfidDevice = new RfidDevice("pre_drill_b_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_pre_drill_schrank", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "spawn_pallett_end_sensor" && isVal)
            {
                Console.WriteLine("Shelves.");
                RfidDevice rfidDevice = new RfidDevice("shelves_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_shelves_production", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "door_type_sensor" && isVal)
            {
                Console.WriteLine("Door Type.");
                RfidDevice rfidDevice = new RfidDevice("door_type_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit sliding = MemoryMap.Instance.GetBit("slidingDoor", MemoryType.Output);
                MemoryBit defaultDoor = MemoryMap.Instance.GetBit("defaultDoor", MemoryType.Output);
                sliding.Value = false;
                defaultDoor.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "lock_sensor" && isVal)
            {
                Console.WriteLine("Lock.");
                RfidDevice rfidDevice = new RfidDevice("lock_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_lock_production", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "elevator_2_end_sensor" && isVal)
            {
                Console.WriteLine("Assemble.");
                RfidDevice rfidDevice = new RfidDevice("assemble_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_assemble", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "extra_stuff_sensor" && isVal)
            {
                Console.WriteLine("Extra Stuff.");
                RfidDevice rfidDevice = new RfidDevice("extra_stuff_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_extra_parts", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "improve_sensor" && isVal)
            {
                Console.WriteLine("Improve.");
                RfidDevice rfidDevice = new RfidDevice("improve_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_improve_quality", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "additional_equipment_sensor" && isVal)
            {
                Console.WriteLine("Additional Equipment.");
                RfidDevice rfidDevice = new RfidDevice("additional_equipment_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit simulateLight = MemoryMap.Instance.GetBit("simulate_additional_equipment", MemoryType.Output);
                simulateLight.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference);
            }
            else if (name == "shape_sensor" && isVal)
            {
                Console.WriteLine("Mirror Shape.");
                RfidDevice rfidDevice = new RfidDevice("shape_rfid");
                int palletReference = await rfidDevice.readValue(0);
                MemoryBit angular = MemoryMap.Instance.GetBit("angular", MemoryType.Output);
                MemoryBit circular = MemoryMap.Instance.GetBit("circular", MemoryType.Output);
                angular.Value = false;
                circular.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(palletReference, "http://host.docker.internal:9033/tasks/completeMirrorTasks");
            }
            else if (name == "pack_trigger_sensor" && isVal)
            {
                MemoryInt readRfid = MemoryMap.Instance.GetInt("pack_rfid_read", MemoryType.Input);
                MemoryBit remover = MemoryMap.Instance.GetBit("pack_remover", MemoryType.Output);
                MemoryBit emitter = MemoryMap.Instance.GetBit("pack_emitter", MemoryType.Output);
                RfidDevice rfidDevice = new RfidDevice("box_rfid");
                int boxSize = await rfidDevice.readValue(1);
                Console.WriteLine("Box Size " + boxSize.ToString());
                remover.Value = true;
                MemoryMap.Instance.Update();
                await Task.Delay(300);
                remover.Value = false;
                emitter.Value = true;
                MemoryMap.Instance.Update();
                await Task.Delay(200);
                remover.Value = true;
                emitter.Value = false;
                MemoryMap.Instance.Update();
                await completeTask(readRfid.Value);
            }
            else if (name == "pusher_sensor" && isVal)
            {
                RfidDevice rfidDevice = new RfidDevice("measurement_rfid");
                int palletReference = await rfidDevice.readValue(0, false);
                await Task.Delay(100);
                Order theOrder = await rfidDevice.getOrder(palletReference);
                try
                {
                    await rfidDevice.writeValue(3, theOrder.LogisticOption);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\nAn error occurred: {ex.Message}");
                }
                await rfidDevice.readValue(3, false);
            }
            else if (name == "logistic_option_1_task_sensor" && isVal)
            {
                Console.WriteLine("Pusher Option 1");
                RfidDevice rfidDevice = new RfidDevice("option_1_rfid");
                int palletReference = await rfidDevice.readValue(0);
                await completeTask(palletReference);
            }
            else if (name == "logistic_option_2_task_sensor" && isVal)
            {
                Console.WriteLine("Pusher Option 2");
                RfidDevice rfidDevice = new RfidDevice("option_2_rfid");
                int palletReference = await rfidDevice.readValue(0);
                await completeTask(palletReference);
            }
            else if (name == "logistic_option_3_task_sensor" && isVal)
            {
                Console.WriteLine("Pusher Option 3");
                RfidDevice rfidDevice = new RfidDevice("option_3_rfid");
                int palletReference = await rfidDevice.readValue(0);
                await completeTask(palletReference);
            }
            else if (name == "pallet_sensor" && isVal)
            {
                MemoryBit conveyor = MemoryMap.Instance.GetBit("pre_palletizer_conveyor", MemoryType.Output);
                RfidDevice rfidDevice = new RfidDevice("palletizer_rfid");
                int palletReference = await rfidDevice.readValue(0);
                var response = await client.GetAsync("http://host.docker.internal:9033/tasks/palletContent");
                string responseContent = await response.Content.ReadAsStringAsync();
                JArray responseArray = JArray.Parse(responseContent);
                int arrayLength = responseArray.Count;
                Console.WriteLine("Array length: " + arrayLength);
                int arrVal = 0;
                int index = 0;
                if (arrayLength == 2)
                {
                    arrVal = responseArray[1].Value<int>();
                    index = 1;
                }
                else if (arrayLength == 1)
                {
                    arrVal = responseArray[0].Value<int>();
                    index = 0;
                }
                if (arrVal != 0)
                {
                    await rfidDevice.writeValue(index, arrVal);
                    Console.WriteLine("Index: " + index.ToString());
                    if (index == 1)
                    {
                        conveyor.Value = true;
                        MemoryMap.Instance.Update();
                    }
                }
            }
            else if (name == "palletizer_elevator_frontLimit" && isVal)
            {
                MemoryBit conveyor = MemoryMap.Instance.GetBit("pre_palletizer_conveyor", MemoryType.Output);
                conveyor.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "spawn_pallet_trigger_sensor_2" && isVal)
            {
                RfidDevice firstRfidDevice = new RfidDevice("palletizer_rfid");
                RfidDevice secondRfidDevice = new RfidDevice("pallet_shipment_rfid");
                int firstBox = await firstRfidDevice.readValue(0);
                int secondBox = await secondRfidDevice.readValue(1);               
            }
            else if (name == "pallet_end_sensor" && isVal)
            {
                MemoryBit stopper = MemoryMap.Instance.GetBit("pre_palletizer_stopper", MemoryType.Output);
                MemoryInt readRfidZero = MemoryMap.Instance.GetInt("palletizer_rfid_read", MemoryType.Input);
                MemoryInt readRfidOne = MemoryMap.Instance.GetInt("pallet_shipment_rfid_read", MemoryType.Input);
                stopper.Value = false;
                MemoryMap.Instance.Update();
                int firstBox = readRfidZero.Value;
                int secondBox = readRfidOne.Value;
                await completeTask(firstBox);
                await completeTask(secondBox);
            }
        }

        static void Instance_ValueChanged(MemoryMap sender, MemoriesChangedEventArgs value)
        {
            foreach (MemoryBit mem in value.MemoriesBit)
            {
                bitValueChange(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
            }
            foreach (MemoryFloat mem in value.MemoriesFloat)
            {
                floatValueChange(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
            }
            foreach (MemoryInt mem in value.MemoriesInt)
            {
                intValueChange(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
            }
        }
    }
}