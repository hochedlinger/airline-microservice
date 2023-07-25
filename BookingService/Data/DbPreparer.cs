using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using AutoMapper;
using BookingService.Communication;
using BookingService.Communication.Http;
using BookingService.Data.Repos;
using BookingService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookingService.Data
{
    public static class DbPreparer
    {
        public static ILogger Logger { get; set; }

        public static void PrepareDb(IApplicationBuilder app, bool isProduction)
        {
            using IServiceScope srvScope = app.ApplicationServices.CreateScope();
            var ctx = srvScope.ServiceProvider.GetService<AppDbContext>();
            if (ctx != null)
            { 
                Logger.LogInformation("Applying migrations");
                try {
                    ctx.Database.Migrate();
                }
                catch (Exception e) {
                    Logger.LogError($"Could not apply migrations: {e.Message}");
                }
            }
        }

        public static async void GetMissingFlights(IApplicationBuilder app)
        {
            using IServiceScope srvScope = app.ApplicationServices.CreateScope();
            var client = srvScope.ServiceProvider.GetService<IFlightClient>();
            var repo = srvScope.ServiceProvider.GetService<IBookingRepo>();
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
                        repo.SaveChanges();
                    }
                }
            }
            catch (Exception e) {
                Logger.LogWarning($"Couldn't check for missing flights: {e.Message}");
            }
        }
    }
}