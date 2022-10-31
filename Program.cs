using System;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;

public class Json
    {
        public string name { get; set; }
        public bool value { get; set; }
        public string type { get; set; }
        public long address { get; set; }
        public DateTime timestamp { get; set; }
    }

class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ProducerConfig()
        {
            BootstrapServers = "localhost:9092" 
        };

        // If serializers are not specified, default serializers from
        // `Confluent.Kafka.Serializers` will be automatically used where
        // available. Note: by default strings are encoded as UTF8.
        using (var p = new ProducerBuilder<Null, string>(config).Build())
        {
            Console.WriteLine("Producer gestartet.");
            try
            {
                Json json = new Json
                {
                    name = "Lorenz",
                    value = true,
                    type = "WOW",
                    address = 1L,
                    timestamp = DateTime.Now
                };
                string body = JsonSerializer.Serialize(json);
                var dr = await p.ProduceAsync("test-topic", new Message<Null, string> { Value=body });
                Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }
    }
}