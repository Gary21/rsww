using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PreferencesService.QueryHandler;
using PreferencesService.Services;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using Serilog;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var rabbitConfig = config.GetSection("rabbitConfig").Get<RabbitConfig>()!;

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

builder.Services.AddSingleton<PublisherServiceBase, PreferencesPublisherService>();
builder.Services.AddHostedService<PreferencesSubscriberService>();
builder.Services.AddHostedService<PreferencesConsumerService>();


var app = builder.Build();
app.Run();