using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TransportRequestService.TransportServiceTests;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Hosting;
using RabbitUtilities.Configuration;
using RabbitUtilities;
using TransportRequestService.RequestHandler;
using TransportRequestService.Repositories;
using Microsoft.EntityFrameworkCore;
using TransportRequestService.Publisher;

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var rabbitConfig = config.GetSection("rabbitConfig").Get<RabbitConfig>()!;


var connectionString = config.GetSection("postgresConfig").GetValue<string>("connectionString");


var builder = WebApplication.CreateBuilder();
builder.Services.Configure<IConfiguration>(config);
builder.Services.AddDbContext<PostgresRepository>(options=> options.UseNpgsql(connectionString), ServiceLifetime.Transient/*Singleton*/);
builder.Services.AddSingleton(logger);
builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory
    {   
        HostName = rabbitConfig.adress, 
        Port = rabbitConfig.port, 
        UserName = rabbitConfig.user, 
        Password = rabbitConfig.password, 
        AutomaticRecoveryEnabled=true
    });
builder.Services.AddSingleton<PublisherServiceBase, TransportPublisherService>();
builder.Services.AddHostedService<TransportRequestHandler>();



//if (Console.ReadLine() == "1")
//{
//    builder.Services.AddHostedService<SubscriberTest>();
builder.WebHost.UseUrls($"http://*:{Random.Shared.Next(15000)}");
//} else 
//{
//    builder.Services.AddSingleton<PublisherServiceBase, TransportPublisherService>();
//    builder.Services.AddHostedService<ReplyService>();
//    builder.Services.AddHostedService<TestPublish>();
//    builder.WebHost.UseUrls("http://*:7136");
//}
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
