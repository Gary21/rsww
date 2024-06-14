using api_gateway.Controllers;
using api_gateway.EventConsumer;
using api_gateway.Publisher;
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
builder.Services.AddSingleton<PublisherServiceBase, GatewayPublisherService>();
builder.Services.AddHostedService<ReplyService>();

builder.Services.AddSingleton<WebSocketService>();
builder.Services.AddHostedService<PreferencesEventConsumer>();
//builder.Services.AddHostedService<ResourceEventConsumer>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
//var webSocketOptions = new WebSocketOptions
//{
//    KeepAliveInterval = TimeSpan.FromMinutes(2)
//};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseWebSockets(/*webSocketOptions*/);
app.UseAuthorization();

app.UseCors("*");

app.MapControllers();

app.Run();