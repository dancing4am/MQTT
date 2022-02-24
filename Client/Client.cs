using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Extensions.ManagedClient;


namespace MQTT_using_MQTTnet.Client
{
    class Client
    {
        static void Main(string[] args)
        {
            // create client option builder
            MqttClientOptionsBuilder builder = new MqttClientOptionsBuilder()
                .WithClientId("SampleClientID")
                .WithTcpServer("localhost", 707);

            // create client option
            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                                    .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                                    .WithClientOptions(builder.Build())
                                    .Build();

            // create client
            IManagedMqttClient client = new MqttFactory().CreateManagedMqttClient();

            // connect handler methods
            client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnect);
            client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnect);
            client.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectFail);
            client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(a => {
                Console.WriteLine("Message received: {0}", a.ApplicationMessage);
            });

            // start connect
            client.StartAsync(options).GetAwaiter().GetResult();

            while (true)
            {
                // send message per second
                string message = "Hello? Anybody there? " + DateTimeOffset.UtcNow;
                client.PublishAsync("SampleClientID/Topic/Hello", message);
                Task.Delay(1000).GetAwaiter().GetResult();
            }
        }

        public static void OnConnect(MqttClientConnectedEventArgs obj)
        {
            Console.WriteLine("Connected to Server");
        }

        public static void OnConnectFail(ManagedProcessFailedEventArgs obj)
        {
            Console.WriteLine("Failed to Connect");
        }

        public static void OnDisconnect(MqttClientDisconnectedEventArgs obj)
        {
            Console.WriteLine("Disconnected from Server");
        }
    }
}
