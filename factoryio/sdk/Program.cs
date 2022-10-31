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

        public class Result
        {
            public string direction { get; set; }
        };

        public class Instance
        {
            public string instance { get; set; }
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
            if (name == "X1_Startsensor" && isVal)
            {
                //hier noch try-catch einbauen!
                var dr = await producer.ProduceAsync("test-topic", new Message<Null, string> { Value="Cool" });
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                Console.WriteLine("Neues Produkt in der Fertigungslinie");

                //###############
                MemoryInt command = MemoryMap.Instance.GetInt("RFID_Command_1", MemoryType.Output);
                MemoryInt readRfid = MemoryMap.Instance.GetInt("RFID_Read_1", MemoryType.Input);
                MemoryInt writeRfid = MemoryMap.Instance.GetInt("RFID_Write_1", MemoryType.Output);
                MemoryInt memory_index = MemoryMap.Instance.GetInt("RFID_Memory_1", MemoryType.Output);
                MemoryInt command_id = MemoryMap.Instance.GetInt("RFID_Command_ID_1", MemoryType.Input);
                MemoryBit rfid = MemoryMap.Instance.GetBit("RFID_Execute_Command_1", MemoryType.Output);
                memory_index.Value = 0;
                command.Value = 3;
                int amount = command_id.Value;
                rfid.Value = true;
                MemoryMap.Instance.Update();
                while (command_id.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                await Task.Delay(100);
                command.Value = 1;
                amount = command_id.Value;
                rfid.Value = true;
                MemoryMap.Instance.Update();
                while (readRfid.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
                await Task.Delay(100);
                Instance instance = new Instance
                {
                    instance = writeRfid.Value.ToString(),
                };
                string body = System.Text.Json.JsonSerializer.Serialize(instance);
                var response = await client.PostAsync("http://abgabe.cs.univie.ac.at:9033/setup", new StringContent(body, Encoding.UTF8, "application/json"));
                var responseString = await response.Content.ReadAsStringAsync();
                Result res = JsonConvert.DeserializeObject<Result>(responseString);
                memory_index.Value = 1;
                writeRfid.Value = int.Parse(res.direction);
                command.Value = 3;
                amount = command_id.Value;
                rfid.Value = true;
                MemoryMap.Instance.Update();
                while (readRfid.Value <= amount)
                {
                    MemoryMap.Instance.Update();
                    Thread.Sleep(16);
                }
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