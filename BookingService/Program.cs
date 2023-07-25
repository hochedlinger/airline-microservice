using System;
using System.IO;
using BookingService.Communication;
using BookingService.Communication.EventProcessor;
using BookingService.Communication.Grpc;
using BookingService.Communication.Http;
using BookingService.Data;
using BookingService.Data.Repos;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

if (builder.Environment.IsProduction()) {
    logger.LogInformation("Production: Using PostgreSQL ...");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetSection("ConnectionStrings")["BSConnStr"]));
}
else {
    logger.LogInformation("Development: Using in-memory database ...");
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseInMemoryDatabase("DevFlights"));
}

builder.Services.AddHostedService<RabbitMqSubscriber>();
builder.Services.AddScoped<IBookingRepo, BookingRepo>();
builder.Services.AddHttpClient<IFlightClient, FlightClient>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IMessageBusClient, RabbitMqClient>();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<BookingGrpcService>();
app.MapGet("/protos/bookings.proto", async ctx =>
{
    await ctx.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

DbPreparer.Logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DbPreparer");
if (app.Environment.IsProduction()) { DbPreparer.PrepareDb(app, app.Environment.IsProduction()); }
DbPreparer.GetMissingFlights(app);

app.Run();
