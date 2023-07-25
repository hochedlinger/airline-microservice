using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CheckInService.Communication.EventProcessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CheckInService.Communication
{
    public class RabbitMqSubscriber : BackgroundService
    {
        private readonly IConfiguration _conf;
        private readonly IEventProcessor _eventProc;
        private readonly ILogger<RabbitMqSubscriber> _logger;
        private IConnection _con;
        private IModel _flightChannel, _bookingChannel;
        private string _flightQueueName, _bookingQueueName;

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

                // Flight channel
                _flightChannel = _con.CreateModel();
                _flightChannel.ExchangeDeclare(exchange: "flights", type: ExchangeType.Fanout);
                _flightQueueName = _flightChannel.QueueDeclare().QueueName;
                _flightChannel.QueueBind(queue: _flightQueueName, exchange: "flights", routingKey: "" );
                _logger.LogInformation("Subscribed to exchange 'flights'");

                // Booking channel
                _bookingChannel = _con.CreateModel();
                _bookingChannel.ExchangeDeclare(exchange: "bookings", type: ExchangeType.Fanout);
                _bookingQueueName = _bookingChannel.QueueDeclare().QueueName;
                _bookingChannel.QueueBind(queue: _bookingQueueName, exchange: "bookings", routingKey: "");
                _logger.LogInformation("Subscribed to exchange 'bookings'");

                _con.ConnectionShutdown += (s, e) => { _logger.LogInformation($"Connection lost: {e.ReplyText}"); };

            }
            catch (Exception e) {
                _logger.LogWarning($"Initialization of RabbitMQ subscriber failed: {e.Message}");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            
            // Flight consumer
            var flightConsumer = new EventingBasicConsumer(_flightChannel);
            flightConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                _logger.LogInformation(msg);
                _eventProc.Process(msg);
            };
            _flightChannel.BasicConsume(queue: _flightQueueName, autoAck: true, consumer: flightConsumer);

            // Booking consumer
            var bookingConsumer = new EventingBasicConsumer(_bookingChannel);
            bookingConsumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                _logger.LogInformation(msg);
                _eventProc.Process(msg);
            };
            _bookingChannel.BasicConsume(queue: _bookingQueueName, autoAck: true, consumer: bookingConsumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_flightChannel.IsOpen) { _flightChannel.Close(); }
            if (_bookingChannel.IsOpen) { _bookingChannel.Close(); }
            if (_con.IsOpen) { _con.Close(); }
        }
    }
}