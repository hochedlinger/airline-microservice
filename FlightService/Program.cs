using System;
using FlightService.Communication;
using FlightService.Data;
using FlightService.Data.Repos;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder.AddConsole());
var logger = loggerFactory.CreateLogger<Program>();

if (builder.Environment.IsProduction()) {
    logger.LogInformation("Production: Using Microsoft SQL Server ...");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["FSConnStr"]));
}
else {
    logger.LogInformation("Development: Using in-memory database ...");
    builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseInMemoryDatabase("DevFlights"));
}

builder.Services.AddScoped<IFlightRepo, FlightRepo>();
builder.Services.AddSingleton<IMessageBusClient, RabbitMqClient>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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

if (app.Environment.IsProduction())
{
    DbPreparer.Logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DbPreparer");
    DbPreparer.PrepareDb(app);
}

app.Run();
