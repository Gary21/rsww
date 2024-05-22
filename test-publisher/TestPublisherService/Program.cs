using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TransportRequestService.TransportServiceTests;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Hosting;
using RabbitUtilities.Configuration;
using RabbitUtilities;
using TestPublisherService.SecondPublisher;

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var rabbitConfig = config.GetSection("rabbitConfig").Get<RabbitConfig>()!;
var rabbitTransactionConfig = config.GetSection("rabbitTransactionConfig").Get<RabbitConfig>()!;

var builder = WebApplication.CreateBuilder();
builder.Services.Configure<IConfiguration>(config);
builder.Services.AddSingleton(logger);
builder.Services.AddKeyedSingleton<IConnectionFactory>("Main", new ConnectionFactory
    {   
        HostName = rabbitConfig.adress, 
        Port = rabbitConfig.port, 
        UserName = "guest", 
        Password = "guest" , 
        AutomaticRecoveryEnabled=true
    });
builder.Services.AddKeyedSingleton<IConnectionFactory>("Transaction", new ConnectionFactory
{
    HostName = rabbitTransactionConfig.adress,
    Port = rabbitTransactionConfig.port,
    UserName = "guest",
    Password = "guest",
    AutomaticRecoveryEnabled = true
});


builder.Services.AddSingleton<TransportPublisherService>();
builder.Services.AddSingleton<Publisher2Service>();


builder.Services.AddHostedService<Reply2Service>();
builder.Services.AddHostedService<ReplyMain>();

builder.Services.AddHostedService<TestPublish>();
builder.WebHost.UseUrls($"http://*:{Random.Shared.Next(15000)}");

builder.Services.AddCors(options =>
    {
        options.AddPolicy("*",
            policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

var app = builder.Build();

app.UseCors("*");
app.Run();
