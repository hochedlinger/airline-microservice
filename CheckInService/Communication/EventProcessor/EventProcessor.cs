using System;
using System.Text.Json;
using AutoMapper;
using CheckInService.Data.Repos;
using CheckInService.DTOs;
using CheckInService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheckInService.Communication.EventProcessor
{
    public class EventProcessor : IEventProcessor
    {
        enum EventType
        {
            FlightCreated,
            FlightUpdated,
            BookingCreated,
            BookingUpdated,
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
                case EventType.FlightUpdated:
                    UpdateFlight(msg);
                    break;
                case EventType.BookingCreated:
                    AddBooking(msg);
                    break;
                case EventType.BookingUpdated:
                    UpdateBooking(msg);
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
                case "FlightUpdated":
                     _logger.LogInformation("Event received: FlightUpdated");
                     return EventType.FlightUpdated;
                case "BookingCreated": 
                    _logger.LogInformation("Event received: BookingCreated");
                    return EventType.BookingCreated;
                case "BookingUpdated":
                     _logger.LogInformation("Event received: BookingUpdated");
                     return EventType.BookingUpdated;
                default:
                    _logger.LogInformation("Event received: Unknown");
                    return EventType.Unknown;
            }
        }

        private void AddFlight(string msg)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICheckInRepo>();

            try {
                var flight = DeserializeAndMap<Flight, FlightPublishDTO>(msg);

                if (!repo.FlightExist(flight.ExternalId)) 
                {
                    repo.CreateFlight(flight);
                    _logger.LogInformation($"Flight added");
                }
            }
            catch (Exception e) {
                _logger.LogWarning($"Couldn't add new flight to database: {e.Message}");
            }
        }

        private void UpdateFlight(string msg)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICheckInRepo>();

            try {
                var flight = DeserializeAndMap<Flight, FlightPublishDTO>(msg);

                if (repo.FlightExist(flight.ExternalId)) {
                    repo.UpdateFlight(flight);

                    _logger.LogInformation($"Flight updated");
                }
            }
            catch (Exception e) {
                _logger.LogWarning($"Couldn't update flight in database: {e.Message}");
            }
        }

        public void AddBooking(string msg)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICheckInRepo>();

            try {   
                var booking = DeserializeAndMap<Booking, BookingPublishDTO>(msg);

                if (!repo.BookingExist(booking.ExternalId)) 
                {
                    repo.CreateBooking(booking);
                    _logger.LogInformation($"Booking added");
                }
            }
            catch (Exception e) {
                _logger.LogWarning($"Couldn't add new booking to database: {e.Message}");
            }
        }

        public void UpdateBooking(string msg)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICheckInRepo>();

            try {
                var booking = DeserializeAndMap<Booking, BookingPublishDTO>(msg);

                if (repo.BookingExist(booking.ExternalId)) 
                {
                    repo.UpdateBooking(booking);
                    _logger.LogInformation($"Booking updated");
                }
            }
            catch (Exception e) {
                _logger.LogWarning($"Couldn't update booking in database: {e.Message}");
            }
        }

        private T DeserializeAndMap<T, U>(string msg) where T : class where U : class
        {
            var publishDTO = JsonSerializer.Deserialize<U>(msg);
            return _mapper.Map<T>(publishDTO);
        }
    }
}