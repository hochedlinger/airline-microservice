using System;
using System.Text.Json;
using AutoMapper;
using BookingService.Data.Repos;
using BookingService.DTOs;
using BookingService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookingService.Communication.EventProcessor
{
    public class EventProcessor : IEventProcessor
    {
        enum EventType
        {
            FlightCreated,
            Unknown
        }

        private readonly ILogger<EventProcessor> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper, ILogger<EventProcessor> logger)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
            _logger = logger;
        }

        public void Process(string msg)
        {
            var eventType = DetermineEvent(msg);

            switch (eventType)
            {
                case EventType.FlightCreated:
                    AddFlight(msg);
                    break;
                default:
                    break;
            }
        }

        private object DetermineEvent(string msg)
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDTO>(msg);

            switch (eventType.Event)
            {
                case "FlightCreated": 
                    _logger.LogInformation("Event received: FlightCreated");
                    return EventType.FlightCreated;
                default:
                    _logger.LogInformation("Event received: Unknown");
                    return EventType.Unknown;
            }
        }

        private void AddFlight(string msg)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBookingRepo>();
            var flightPublishDTO = JsonSerializer.Deserialize<FlightPublishDTO>(msg);

            try {
                var flight = _mapper.Map<Flight>(flightPublishDTO);

                if (!repo.FlightExist(flight.ExternalId)) {
                    repo.CreateFlight(flight);
                    repo.SaveChanges();

                    _logger.LogInformation($"Flight added");
                }
            }
            catch (Exception e) {
                _logger.LogWarning($"Couldn't add new flight to database: {e.Message}");
            }
        }
    }
}