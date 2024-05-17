using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitUtilities.Configuration;
using Serilog;
using TransportQueryService.QueryHandler;
using TransportQueryService.Repositories;

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
        UserName = "guest", 
        Password = "guest" , 
        AutomaticRecoveryEnabled=true
    });
builder.Services.AddHostedService<TransportQueryHandler>();
builder.WebHost.UseUrls("http://*:7134");
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
