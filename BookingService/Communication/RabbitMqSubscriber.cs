using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BookingService.Communication.EventProcessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BookingService.Communication
{
    public class RabbitMqSubscriber : BackgroundService
    {
        private readonly IConfiguration _conf;
        private readonly IEventProcessor _eventProc;
        private readonly ILogger<RabbitMqSubscriber> _logger;
        private IConnection _con;
        private IModel _channel;
        private string _queueName;

        public RabbitMqSubscriber(IConfiguration conf, IEventProcessor eventProc, ILogger<RabbitMqSubscriber> logger)
        {
            _conf = conf;
            _eventProc = eventProc;
            _logger = logger;

            Init();
        }

        private void Init()
        {
            try {
                var factory = new ConnectionFactory() { 
                    HostName = _conf["RabbitMQ:Host"], 
                    Port = int.Parse(_conf["RabbitMQ:Port"]) 
                };
                _con = factory.CreateConnection();
                _channel = _con.CreateModel();
                _channel.ExchangeDeclare(exchange: "flights", type: ExchangeType.Fanout);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(
                    queue: _queueName,
                    exchange: "flights",
                    routingKey: ""
                );

                _logger.LogInformation("Subscribed to exchange 'flights'");

                _con.ConnectionShutdown += (s, e) => { _logger.LogInformation($"Connection lost: {e.ReplyText}"); };

            }
            catch (Exception e) {
                _logger.LogWarning($"Initialization of RabbitMQ subscriber failed: {e.Message}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);

                _logger.LogInformation(msg);

                _eventProc.Process(msg);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _con.Close();
            }
        }
    }
}