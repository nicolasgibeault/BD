using MQTTnet;
using MQTTnet.Server;
using MQTTnet.Server.Status;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace PFE.Broker
{
    public class Server
    {
        private Server() { }

        private IMqttServer _mqttServer;
        private static Server _server;
        private static int _messageCount;

        public static Server GetServer
        {
            get 
            {
                if (_server == null)
                    _server = new Server();

                return _server;
            }
        }

        public void StartServer(int port)
        {
            MqttServerOptionsBuilder options = new MqttServerOptionsBuilder()
                                                 .WithDefaultEndpoint()
                                                 .WithDefaultEndpointPort(port)
                                                 .WithConnectionValidator(OnNewConnection)
                                                 .WithApplicationMessageInterceptor(OnNewMessage);
    
            _mqttServer = new MqttFactory().CreateMqttServer();
 
            _mqttServer.StartAsync(options.Build()).GetAwaiter().GetResult();

            Console.ReadLine();
        }

        public async void StopServer()
        {
            if (_mqttServer != null)
            {
                await _mqttServer.StopAsync();
            }
        }

        public static void OnNewConnection(MqttConnectionValidatorContext context)
        {
            Log.Logger.Information(
                    "New connection: ClientId = {clientId}, Endpoint = {endpoint}",
                    context.ClientId,
                    context.Endpoint);
        }

        public static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
        {
            var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

            _messageCount++;

            Log.Logger.Information(
                "MessageId: {MessageCounter} - TimeStamp: {TimeStamp} -- Message: ClientId = {clientId}, Topic = {topic}, Payload = {payload}, QoS = {qos}, Retain-Flag = {retainFlag}",
                _messageCount,
                DateTime.Now,
                context.ClientId,
                context.ApplicationMessage?.Topic,
                payload,
                context.ApplicationMessage?.QualityOfServiceLevel,
                context.ApplicationMessage?.Retain);
        }

    }
}
