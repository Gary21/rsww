using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Hosting;
using RabbitUtilities.Configuration;
using RabbitUtilities;
using OrderService.RequestHandler;
using OrderService.Repositories;
using Microsoft.EntityFrameworkCore;
using OrderService.Publisher;

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var rabbitConfig = config.GetSection("rabbitConfig").Get<RabbitConfig>()!;


var connectionString = config.GetSection("postgresConfig").GetValue<string>("connectionString");


var builder = WebApplication.CreateBuilder();
builder.Services.Configure<IConfiguration>(config);
builder.Services.AddDbContext<PostgresRepository>(options => options.UseNpgsql(connectionString), ServiceLifetime.Transient/*Singleton*/);
builder.Services.AddSingleton(logger);
builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory
{
    HostName = rabbitConfig.adress,
    Port = rabbitConfig.port,
    UserName = rabbitConfig.user,
    Password = rabbitConfig.password,
    AutomaticRecoveryEnabled = true
});
builder.Services.AddSingleton<PublisherServiceBase, OrderPublisherService>();
builder.Services.AddHostedService<OrderRequestHandler>();



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
