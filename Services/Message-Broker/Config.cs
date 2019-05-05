using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace Message_Broker
{
    public class Config : IDisposable
    {
        private ConfigureServer _cs;

        public async Task ConfigureServer()
        {
            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithConnectionBacklog(100)
                .WithDefaultEndpointPort(1884)
                .WithConnectionValidator(ConnectionValidator);

            _cs = new ConfigureServer(optionsBuilder);
            _cs.MessageReceivedEventHandler(MqttApplicationMessageReceived);
            await _cs.StartServing();
        }

        private void MqttApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("\nThere is a fucking message comes around!!.");
            Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
            Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
            Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
            Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
            Console.WriteLine($"+ ClientId = {e.ClientId}");
        }

        private void ConnectionValidator(MqttConnectionValidatorContext c)
        {
            if (c.ClientId.Length < 10)
            {

            }
            if (!c.Username.Equals("pi"))
            {
                c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                return;
            }
            if (!c.Password.Equals("mj06174551gh"))
            {
                c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                return;
            }
            c.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
        }

        public async Task Stop() => await _cs.StopServing();
        public async Task Publish(MqttApplicationMessageBuilder message) => await _cs.PublishMessage(message);

        

        public void Dispose()
        {
            _cs.Dispose();
        }
    }

    public class ConfigureServer : IDisposable
    {
        private IMqttServer MqttServer { get; set; }
        private MqttServerOptionsBuilder Options { get; set; }
        public ConfigureServer(MqttServerOptionsBuilder options)
        {
            Options = options;
            MqttServer = new MqttFactory().CreateMqttServer();
        }

        public void Dispose() => GC.SuppressFinalize(this);
        public void MessageReceivedEventHandler(EventHandler<MqttApplicationMessageReceivedEventArgs> func) =>
            MqttServer.ApplicationMessageReceived += func;
        public async Task StartServing() => await MqttServer.StartAsync(Options.Build());
        public async Task StopServing() => await MqttServer.StopAsync();
        public async Task PublishMessage(MqttApplicationMessageBuilder message)
        {
            if (message == null) throw new ArgumentNullException($"The argument named {nameof(message)} couldn't be null");
            await MqttServer.PublishAsync(message.Build());
        } 
    }
}
