using System;
using System.Text;
using System.Text.Json;
using FlightService.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace FlightService.Communication
{
    public class RabbitMqClient : IMessageBusClient
    {
        private readonly IConfiguration _conf;
        private readonly IConnection _con;
        private readonly IModel _channel;

        private readonly ILogger<RabbitMqClient> _logger;

        public RabbitMqClient(IConfiguration conf, ILogger<RabbitMqClient> logger)
        {
            _conf = conf;
            _logger = logger;

            try {
                var factory = new ConnectionFactory() { 
                    HostName = _conf["RabbitMQ:Host"], 
                    Port = int.Parse(_conf["RabbitMQ:Port"]) 
                };

                _con = factory.CreateConnection();
                _channel = _con.CreateModel();
                _channel.ExchangeDeclare(exchange: "flights", type: ExchangeType.Fanout);
                _con.ConnectionShutdown += OnConnectionShutdown;

                _logger.LogInformation("RabbitMQ connection established");
            }
            catch (Exception e) {
                _logger.LogError($"RabbitMQ connection failed: {e.Message}");
            }
        }

        public void Publish(FlightPublishDTO flightPublishDTO)
        {
            if (!_con.IsOpen) {
                _logger.LogWarning("RabbitMQ connection closed, publishing failed");
                return;
            }

            var msg = JsonSerializer.Serialize(flightPublishDTO);
            var body = Encoding.UTF8.GetBytes(msg);

            _channel.BasicPublish(exchange: "flights", routingKey: "", basicProperties: null, body: body );

            _logger.LogInformation($"Published message to RabbitMQ: {msg}");
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogWarning($"RabbitMQ connection shutdown: {e.ReplyText}");
        }

        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _con.Close();
            }

            _logger.LogInformation("RabbitMQ client disposed");
        }
        
    }
}