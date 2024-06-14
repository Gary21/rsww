using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Hosting;
using RabbitUtilities.Configuration;
using RabbitUtilities;
using Microsoft.EntityFrameworkCore;
using SimulationService;

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var rabbitConfig = config.GetSection("rabbitConfig").Get<RabbitConfig>()!;
var connectionString = config.GetValue<string>("postgresConfig:connectionString");//.GetValue<string>("connectionString");

var builder = WebApplication.CreateBuilder();
builder.Services.Configure<IConfiguration>(config);
builder.Services.AddSingleton(logger);
builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory
{
    HostName = rabbitConfig.adress,
    Port = rabbitConfig.port,
    UserName = rabbitConfig.user,
    Password = rabbitConfig.password,
    AutomaticRecoveryEnabled = true
});

builder.Services.AddSingleton<PublisherServiceBase, SimulationPublisher>();
builder.Services.AddHostedService<ReplyService>();
builder.Services.AddHostedService<SimulationBackgroundService>();

builder.WebHost.UseUrls($"http://*:{Random.Shared.Next(15000)}");

var app = builder.Build();

app.Run();
