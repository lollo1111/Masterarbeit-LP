using System;
using System.Threading;
using System.Text.Json;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

//dotnet add package Newtonsoft.Json --version 13.0.1

namespace EngineIO.Samples
{
    class Program
    {
        public static ProducerConfig config = new ProducerConfig()
        {
            BootstrapServers = "localhost:9092"
        };

        public static IProducer<Null, string> producer = new ProducerBuilder<Null, string>(config).Build();

        private static readonly HttpClient client = new HttpClient();

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

        static void Main(string[] args)
        {
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
            // FactoryEvent factoryevent = new FactoryEvent
            // {
            //     name = name,
            //     value = isVal,
            //     type = type,
            //     address = address,
            //     timestamp = DateTime.Now
            // };
            // string body = System.Text.Json.JsonSerializer.Serialize(factoryevent);
            // Console.WriteLine(body);
            if (name == "start_sensor" && isVal)
            {
                // Produkt wurde platziert
                var response = await client.GetAsync("http://host.docker.internal:9033/start/finished");
            }
            else if (name == "start_rfid_sensor" && isVal)
            {
                //hier noch try-catch einbauen!
                var dr = await producer.ProduceAsync("test-topic", new Message<Null, string> { Value="Cool" });
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                Console.WriteLine("Neues Produkt in der Fertigungslinie");

                //###############
                MemoryInt command = MemoryMap.Instance.GetInt("start_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("start_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("start_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("start_rfid_memoryindex", MemoryType.Output);
                //number of executed commands --> id
                MemoryInt command_id = MemoryMap.Instance.GetInt("start_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("start_rfid_execute", MemoryType.Output);
                // Speicher an Stelle 0 --> Write Command: 3
                // memory_index.Value = 0;
                // command.Value = 3;
                // int amount = command_id.Value;
                // rfid.Value = true;
                // MemoryMap.Instance.Update();
                // while (command_id.Value <= amount)
                // {
                //     MemoryMap.Instance.Update();
                //     Thread.Sleep(16);
                // }
                // await Task.Delay(100);
                memory_index.Value = 0;
                command.Value = 1;
                int amount = command_id.Value;
                rfid.Value = true;
                // MemoryMap.Instance.Update();
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Reference reference = new Reference
                {
                    reference = readRfid.Value.ToString(),
                };
                var order = await client.GetAsync("http://host.docker.internal:9033/start/details");
                var orderString = await order.Content.ReadAsStringAsync();
                Order theOrder = JsonConvert.DeserializeObject<Order>(orderString);
                Console.WriteLine(theOrder.Id);
                rfid.Value = false;

                // 1. Referenznummer auf Index 0 hinterlegen
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                amount = command_id.Value;
                writeRfid.Value = readRfid.Value;
                command.Value = 3;
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                string body = System.Text.Json.JsonSerializer.Serialize(reference);
                var response = await client.PostAsync("http://host.docker.internal:9033/start/setup", new StringContent(body, Encoding.UTF8, "application/json"));


                // 2. Produkt auf Index 1 hinterlegen
                memory_index.Value = 1;
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
                amount = command_id.Value;
                if (theOrder.Product == "schrank")
                {
                    writeRfid.Value = 0;
                }
                else if (theOrder.Product == "schreibtisch")
                {
                    writeRfid.Value = 1;
                }
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }

                // 3. Wenn Schrank, dann Türtyp, andernfalls Tischstil auf Index 2 hinterlegen
                memory_index.Value = 2;
                rfid.Value = false;
                MemoryMap.Instance.Update();
                await Task.Delay(100);
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
                }
                else if (theOrder.TableStyle != null)
                {
                    if (theOrder.TableStyle == "classic")
                    {
                        writeRfid.Value = 0;
                    }
                    else
                    {
                        writeRfid.Value = 1;
                    }
                }
                rfid.Value = true;
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                rfid.Value = false;
                MemoryMap.Instance.Update();
                // var responseString = await response.Content.ReadAsStringAsync();
                
                // Result res = JsonConvert.DeserializeObject<Result>(responseString);
                // memory_index.Value = 1;
                // writeRfid.Value = int.Parse(res.direction);
                // command.Value = 3;
                // amount = command_id.Value;
                // rfid.Value = true;
                // MemoryMap.Instance.Update();
                // while (readRfid.Value <= amount)
                // {
                //     MemoryMap.Instance.Update();
                //     Thread.Sleep(16);
                // }
            }
            else if (name == "direction_rfid_sensor" && isVal)
            {
                MemoryInt command = MemoryMap.Instance.GetInt("direction_rfid_command", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("direction_rfid_read", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("direction_rfid_write", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("direction_rfid_memoryindex", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("direction_rfid_id", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("direction_rfid_execute", MemoryType.Output);
                memory_index.Value = 1;
                command.Value = 2;
                int amount = command_id.Value;
                rfid.Value = true;
                // MemoryMap.Instance.Update();
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Console.WriteLine("Result:" + readRfid.Value.ToString());
                rfid.Value = false;
                MemoryMap.Instance.Update();
            }
            else if (name == "secondtrigger" && isVal)
            {
                Console.WriteLine("Set to read");
                MemoryInt command = MemoryMap.Instance.GetInt("RFID_Command_2", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt(5, MemoryType.Input);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("RFID_Memory_2", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("RFID_Command_ID_2", MemoryType.Input);
                int amount = command_id.Value;
                command.Value = 2;
                memory_index.Value = 1;
                MemoryBit rfid = MemoryMap.Instance.GetBit("RFID_Execute_Command_2", MemoryType.Output);
                rfid.Value = true;
                MemoryMap.Instance.Update();
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                Console.WriteLine(string.Format("Direction: {0}.", readRfid.Value));
                MemoryInt direction = MemoryMap.Instance.GetInt("direction", MemoryType.Output);
                direction.Value = readRfid.Value;
                MemoryMap.Instance.Update();
            }
            else if ((name == "end1" || name == "end2") && isVal)
            {
                MemoryInt direction = MemoryMap.Instance.GetInt("direction", MemoryType.Output);
                direction.Value = 0;
                MemoryMap.Instance.Update();
            }
        }

        static void Instance_ValueChanged(MemoryMap sender, MemoriesChangedEventArgs value)
        {
            //Display any changed MemoryBit
            foreach (MemoryBit mem in value.MemoriesBit)
            {
                createObj(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
            }

            //Display any changed MemoryFLoat
            foreach (MemoryFloat mem in value.MemoriesFloat)
            {
                createObj(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
                // Console.WriteLine(string.Format("{0} value changed to: {1}, Address: {2} {3}", mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address));
            }

            //Display any changed MemoryInt
            foreach (MemoryInt mem in value.MemoriesInt)
            {
                createObj(mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address);
                // Console.WriteLine(string.Format("{0} value changed to: {1}, Address: {2} {3}", mem.Name, mem.Value.ToString(), mem.MemoryType.ToString(), mem.Address));
            }
        }
    }
}