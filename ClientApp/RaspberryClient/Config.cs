using System;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using System.Threading.Tasks;

namespace RaspberryClient
{
    public class ClientConfigure : IDisposable
    {
        private IMqttClient Client {get;set;}
        private IMqttClientOptions Option {get;set;}
        public ClientConfigure(MqttClientOptionsBuilder option)
        {
            Client = new MqttFactory().CreateMqttClient();
            Option = option.Build();
        }

        public async Task Connect()
        {
            await Client.ConnectAsync(Option);
        }

        public void DisconnectedEventHandler()
        {
            Client.Disconnected += async (c,e) =>
            {
                Console.WriteLine($"Error: {e.Exception.Message}");
                await Task.Delay(TimeSpan.FromSeconds(5));
                await Client.ConnectAsync(Option);
            };
        }

        public void MessageReceivedEventHandler(EventHandler<MqttApplicationMessageReceivedEventArgs> func) =>
            Client.ApplicationMessageReceived += func;


        public async Task PublishMessage(MqttApplicationMessageBuilder message)
        {
            if (message == null) throw new ArgumentNullException($"The argument named {nameof(message)} couldn't be null");
            await Client.PublishAsync(message.Build());
        }

        public async Task Subscribe(TopicFilterBuilder topic)
        {
            if (topic == null) throw new ArgumentNullException($"The argument named {nameof(topic)} couldn't be null");
            await Client.SubscribeAsync(topic.Build());
        }

        
        public void Dispose()
        {
            Client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }


    public class Config : IDisposable
    {
        private ClientConfigure _cc;
        public async Task ConfigMqttAsync()
        {
            var options = new MqttClientOptionsBuilder()
                .WithClientId("pi-Lamp-connectivity")
                .WithTcpServer("localhost",1884)
                .WithCredentials("pi", "mj06174551gh")
                //.WithTls()
                .WithCleanSession();


            _cc = new ClientConfigure(options);
            _cc.DisconnectedEventHandler();
            _cc.MessageReceivedEventHandler(MqttApplicationMessageReceived);
            await _cc.Connect();
        }

        private void MqttApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("There is a fucking message comes around!!.");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine($"+ ClientId = {e.ClientId}");
        }

        public async Task Publish(MqttApplicationMessageBuilder message) => await _cc.PublishMessage(message);
        public async Task Subscribe(TopicFilterBuilder topic) => await _cc.Subscribe(topic);
        public void Dispose() => _cc.Dispose();
    }
}