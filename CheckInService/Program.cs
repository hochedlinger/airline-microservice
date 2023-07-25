using System;
using CheckInService.Communication;
using CheckInService.Communication.EventProcessor;
using CheckInService.Communication.Grpc;
using CheckInService.Communication.Http;
using CheckInService.Data;
using CheckInService.Data.Repos;
using CheckInService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddScoped<ICheckInRepo, CheckInRepo>();
builder.Services.AddScoped<IBookingClient, BookingClient>();
builder.Services.AddHttpClient<IFlightClient, FlightClient>();
builder.Services.AddHostedService<RabbitMqSubscriber>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Truncate collections for development
    using (var scope = app.Services.CreateScope())
    {
        var ctx = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        ctx.Flights.DeleteMany(FilterDefinition<Flight>.Empty);
        ctx.Bookings.DeleteMany(FilterDefinition<Booking>.Empty);
        ctx.CheckIns.DeleteMany(FilterDefinition<CheckIn>.Empty);
    }
}

app.UseAuthorization();

app.MapControllers();

DbPreparer.Logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DbPreparer");
DbPreparer.GetMissingFlights(app);
DbPreparer.GetMissingBookings(app);

app.Run();
