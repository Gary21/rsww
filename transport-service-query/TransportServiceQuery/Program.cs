using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Microsoft.AspNetCore.Hosting;
using RabbitUtilities.Configuration;
using TransportQueryService.QueryHandler;
using Microsoft.EntityFrameworkCore;
using TransportQueryService.Repositories;

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();

var rabbitConfig = config.GetSection("rabbitConfig").Get<RabbitConfig>()!;
var connectionString = config.GetValue<string>("postgresConfig:connectionString");//.GetValue<string>("connectionString");


var builder = WebApplication.CreateBuilder();
builder.Services.Configure<IConfiguration>(config);
builder.Services.AddDbContextFactory<PostgresRepository>(options => options.UseNpgsql(connectionString));
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
//builder.WebHost.UseUrls("http://*:7134");
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
