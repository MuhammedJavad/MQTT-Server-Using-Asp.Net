using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace RaspberryClient
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Starting fucking client...");
            using (var config = new Config())
            {
                await config.ConfigMqttAsync();
                await TemporaryMethod(config);
            }

            Console.ReadLine();
        }

        private static async Task TemporaryMethod(Config c)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic("pi-Lamp")
                .WithPayload(DateTime.Now.ToString(CultureInfo.InvariantCulture))
                .WithExactlyOnceQoS()
                .WithRetainFlag(true);

            await c.Publish(message);
            
            // Subscribe to a topic
//            var topic = new TopicFilterBuilder().WithTopic("pi-lamp");
//            await c.Subscribe(topic);

//            Console.WriteLine("### SUBSCRIBED ###");
//            var count = 0;
//            while (true)
//            {
//                count++;
//                var message = new MqttApplicationMessageBuilder()
//                    .WithTopic(count.ToString())
//                    .WithPayload(DateTime.Now.ToString(CultureInfo.InvariantCulture))
//                    .WithExactlyOnceQoS()
//                    .WithRetainFlag(true);
//
//                await c.Publish(message);
////                await c.Subscribe(topic);
//                Thread.Sleep(1000);
//                if (count == 100001) count = 0;
//            }
        }
    }
}
