using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;

namespace Message_Broker
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting fucking Server....");
            using (var config = new Config())
            {
                await config.ConfigureServer();
                //await TemporaryMethod(c: config);
                Console.ReadLine();
                Console.WriteLine("Server is about to stop");
                await config.Stop();
            }
        }

        private static async Task TemporaryMethod(Config c)
        {
            var count = 0;
            while (true)
            {
                count++;
                var message = new MqttApplicationMessageBuilder()
                    .WithTopic(count.ToString())
                    .WithPayload(DateTime.Now.ToString(CultureInfo.InvariantCulture))
                    .WithExactlyOnceQoS()
                    .WithRetainFlag(true);

                await c.Publish(message);
                Thread.Sleep(1000);
                if (count == 100001) count = 0;
            }
        }
    }
}
