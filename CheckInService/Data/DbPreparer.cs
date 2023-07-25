

using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoMapper;
using CheckInService.Communication.Grpc;
using CheckInService.Communication.Http;
using CheckInService.Data.Repos;
using CheckInService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CheckInService.Data
{
    public static class DbPreparer
    {
        public static ILogger Logger { get; set; }

        public static async void GetMissingFlights(IApplicationBuilder app)
        {
            using IServiceScope srvScope = app.ApplicationServices.CreateScope();
            var client = srvScope.ServiceProvider.GetService<IFlightClient>();
            var repo = srvScope.ServiceProvider.GetService<ICheckInRepo>();
            var mapper = srvScope.ServiceProvider.GetService<IMapper>();

            try {
                IEnumerable<int> ids = await client.GetAllFlightIds();

                foreach(var id in ids)
                {
                    if (!repo.FlightExist(id))
                    {
                        var flightDetailsDTO = await client.GetFlightById(id);
                        var flight = mapper.Map<Flight>(flightDetailsDTO);
                        repo.CreateFlight(flight);
                    }
                }
            }
            catch (Exception e) {
                Logger.LogWarning($"Couldn't check for missing flights: {e.Message}");
            }
        }

        public static void GetMissingBookings(IApplicationBuilder app) 
        {
            using IServiceScope srvScope = app.ApplicationServices.CreateScope();
            var client = srvScope.ServiceProvider.GetService<IBookingClient>();
            var repo = srvScope.ServiceProvider.GetService<ICheckInRepo>();

            try {
                var bookings = client.GetAllBookings();
                if (bookings == null) { return; }

                foreach(var booking in bookings)
                {
                    if (!repo.BookingExist(booking.ExternalId))
                    {
                        repo.CreateBooking(booking);
                    }
                }

            }
            catch (Exception e) {
                Logger.LogWarning($"Couldn't check for missing bookings: {e.Message}");
            }
        }

    }
}