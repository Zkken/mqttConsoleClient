using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Options;

namespace MqttConsoleClient
{
    internal class Program
    {
        private static async Task Main()
        {
            var client = new MqttConsoleClient(GetConsoleInputOptions());

            await client.StartAsync();

            await client.SubscribeAsync("hello/world", "hello/test");

            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                await client.PublishAsync(GetConsoleInputMessage());

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            await client.DisconnectAsync();
        }

        private static MqttApplicationMessage GetConsoleInputMessage()
        {
            return new MqttApplicationMessageBuilder()
                .WithExactlyOnceQoS()
                .WithPayload(ReadFromConsole("Message: "))
                .WithTopic(ReadFromConsole("Topic: "))
                .Build();
        }

        private static IMqttClientOptions GetConsoleInputOptions(string server = "localhost", int port = 1883)
        {
            return new MqttClientOptionsBuilder()
                .WithClientId(ReadFromConsole("Client id: "))
                .WithTcpServer(server, port)
                .Build();
        }

        private static string ReadFromConsole(string label = "")
        {
            Console.Write(label);
            return Console.ReadLine();
        }
    }
}
