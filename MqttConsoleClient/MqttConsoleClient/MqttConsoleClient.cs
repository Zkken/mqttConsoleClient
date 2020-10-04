using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace MqttConsoleClient
{
    public class MqttConsoleClient
    {
        private readonly IMqttClient _client;
        private readonly IMqttClientOptions _clientOptions;

        public MqttConsoleClient(IMqttClientOptions clientOptions)
        {
            _clientOptions = clientOptions;
            _client = new MqttFactory().CreateMqttClient();
            RegisterHandlers();
        }

        public async Task PublishAsync(MqttApplicationMessage message)
        {
            await _client.PublishAsync(message);
        }

        public async Task StartAsync()
        {
            await _client.ConnectAsync(_clientOptions, CancellationToken.None);
        }

        public async Task SubscribeAsync(params string[] topics)
        {
            await Task.WhenAll(topics.Select(async topic => await _client.SubscribeAsync(topic)));

            Console.WriteLine($"Subscribed to {string.Join(',', topics)} topics");
        }

        public async Task DisconnectAsync()
        {
            await _client.DisconnectAsync();
        }

        private void RegisterHandlers()
        {
            _client.UseConnectedHandler(e =>
            {
                Console.WriteLine("### Connected with server ###");
            });

            _client.UseApplicationMessageReceivedHandler(e =>
            {
                Console.WriteLine();
                Console.WriteLine("# Received application message #");
                Console.WriteLine($"From topic: {e.ApplicationMessage.Topic}");
                Console.WriteLine($"Payload: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"Qos: {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"Retain: {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });

            _client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("### Disconnected from server ###");
            });
        }
    }
}
