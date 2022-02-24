using MQTTnet;
using MQTTnet.Server;
using System;
using System.Text;

namespace MQTT_using_MQTTnet.Broker
{
    class Server
    {
        private static int MessageCount = 0;    // used as message sequence number

        static void Main(string[] args)
        {
            // create server option builder
            MqttServerOptionsBuilder options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(707)
                .WithConnectionValidator(OnNewConnection)
                .WithApplicationMessageInterceptor(OnNewMessage);

            // create server
            IMqttServer server = new MqttFactory().CreateMqttServer();

            // start service
            server.StartAsync(options.Build()).GetAwaiter().GetResult();
            Console.ReadLine(); // when any input, stop to debug disconnect
        }

        public static void OnNewConnection(MqttConnectionValidatorContext context)
        {
            // display new client info
            Console.WriteLine("New Client: ClientID = {0}, Endpoint = {1}, CleanSession = {2}",
                context.ClientId, context.Endpoint, context.CleanSession);
        }

        public static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            // prepare message content
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);
            MessageCount++;

            // display message in format
            Console.WriteLine("Msg#: {0} - TimeStamp: {1} -- Message: ClientID = {2}, Topic = {3}, Payload = {4}, QoS = {5}, Retain-Flag = {6}",
                MessageCount, DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain
                );
        }
    }
}
