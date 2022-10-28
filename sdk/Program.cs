using System;
using System.Threading;
using System.Text.Json;
using System.Text;
using System.Net.Http;

namespace EngineIO.Samples
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
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
            //Registering on the events
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
            FactoryEvent factoryevent = new FactoryEvent
            {
                name = name,
                value = isVal,
                type = type,
                address = address,
                timestamp = DateTime.Now
            };
            string body = JsonSerializer.Serialize(factoryevent);
            Console.WriteLine(body);
            //var responseString = await client.GetStringAsync("http://localhost:3000/event");
            var response = await client.PostAsync("http://localhost:3000/event", new StringContent(body, Encoding.UTF8, "application/json"));
            //var responseString = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseString);
            if (name == "X1_Startsensor" && isVal)
            {
                var random = new Random();
                var randomBool = random.Next(2) == 1;
                Console.WriteLine(string.Format("Neues Produkt in der Fertigungslinie, zugewiesener Wert: {0}.", randomBool));
                MemoryInt content = MemoryMap.Instance.GetInt("RFID_Write_1", MemoryType.Output);
                MemoryInt command = MemoryMap.Instance.GetInt("RFID_Command_1", MemoryType.Output);
                if (randomBool) {
                    content.Value = 11;
                } else {
                    content.Value = 0;
                }
                command.Value = 3;
                MemoryMap.Instance.Update();
                Console.WriteLine(string.Format("Value: {0}", content.Value));
            }

            else if (name == "secondtrigger" && isVal) {
                Console.WriteLine("Set to read");
                MemoryInt command = MemoryMap.Instance.GetInt("RFID_Command_2", MemoryType.Output);
                command.Value = 2;
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