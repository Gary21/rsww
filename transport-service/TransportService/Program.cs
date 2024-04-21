using Microsoft.Extensions.Configuration;
using Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using TransportService.TransportService;
using RabbitMQ.Client;
using TransportService.Configuration;
using Microsoft.Extensions.Hosting;
using TransportService;
using Microsoft.AspNetCore.Hosting;

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
MessageQueueConfig messageQueueConfig = config.GetSection("messageQueueConfig").Get<MessageQueueConfig>()!;

var builder = WebApplication.CreateBuilder();
builder.Services.Configure<IConfiguration>(config);
builder.Services.AddSingleton(logger);
builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory
    { HostName = messageQueueConfig.adress, Port = messageQueueConfig.port, UserName = "guest", Password = "guest" });

if (Console.ReadLine() == "1")
{
    builder.Services.AddHostedService<TransportMessageQueueHandler>();
    builder.WebHost.UseUrls("http://*:7135");
} else 
{
    builder.Services.AddSingleton<PublisherServiceBase, TransportPublisherTest>();
    builder.Services.AddHostedService<ReplyService>();
    builder.Services.AddHostedService<TestPublish>();
    builder.WebHost.UseUrls("http://*:7136");
}
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
