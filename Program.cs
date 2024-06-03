using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace AQI_Producer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            List<long> pollNumbers = [2442, 3254, 1248, 3567, 15497, 55479, 74, 974, 546, 7912, 8794, 1247, 1246, 79461, 39, 313, 544, 68, 154, 8478];

            var configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

            var config = new ProducerConfig
            {
                BootstrapServers = $"{configuration["BootstrapService:Server"]}:{configuration["BootstrapService:Port"]}"
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                while (true)
                {
                    List<AqiModel> dataModels = new List<AqiModel>();

                    foreach (var v in pollNumbers)
                    {
                        dataModels.Add(new AqiModel
                        {
                            PollNumber = v,

                            PM10 = randomDataGenerator(0,430),
                            PM2_5 = randomDataGenerator(0,400),
                            NO2 = randomDataGenerator(0,748),
                            O3 = randomDataGenerator(0,430),
                            CO = randomDataGenerator(0,430),
                            SO2 = randomDataGenerator(0,430),
                            NH3 = randomDataGenerator(0,430),
                            PB = randomDataGenerator(0,430),

                            Temperature= randomDataGenerator(25, 55),
                            Wind = randomDataGenerator(25, 90),
                            Pressure = randomDataGenerator(20, 35),
                            Precip = randomDataGenerator(0, 1),
                            Visibility = randomDataGenerator(0, 8),
                            Humidity = randomDataGenerator(40, 70),
                            Uv = randomDataGenerator(0, 2),
                            Gust = randomDataGenerator(20, 40),
                            Feelslike = randomDataGenerator(30, 55),

                            XCoordinate = "23.0256631",
                            YCoordinate = "72.5073311",
                            Area= "Iskcon Cross Road",
                            City= "Ahmedabad",
                            State= "Gujarat",

                            TimeStamp = DateTime.Now,
                        });
                    }

                    try
                    {
                        await producer.ProduceAsync(configuration["BootstrapService:Topic"], new Message<Null, string> { Value = JsonConvert.SerializeObject(dataModels) });
                        await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(dataModels));
                        await Console.Out.WriteLineAsync();
                    }
                    catch (ProduceException<Null, string> e)
                    {
                        Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                    }
                    Thread.Sleep(1000);
                }
            }
        }
        private static int randomDataGenerator(int min,int max)
        {
            Random random = new Random();
            return random.Next(min, max + 1);
        }
    }
}