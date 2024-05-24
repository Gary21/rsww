using CatalogRequestService.QueryPublishers;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using Serilog;
using ILogger = Serilog.ILogger;
using CatalogRequestService.RequestHandlers;
using CatalogRequestService.RequestPublishers;

ILogger logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").AddEnvironmentVariables().Build();
var rabbitConfig = config.GetSection("rabbitConfig").Get<RabbitConfig>()!;
var transactionRabbitConfig = config.GetSection("transactionRabbitConfig").Get<RabbitConfig>()!;

var connectionString = config.GetSection("postgresConfig").GetValue<string>("connectionString");



var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<IConfiguration>(config);
builder.Services.AddSingleton(logger);

builder.Services.AddSingleton<IConnectionFactory>(new ConnectionFactory
{
    HostName = rabbitConfig.adress,
    Port = rabbitConfig.port,
    UserName = "guest",
    Password = "guest",
    AutomaticRecoveryEnabled = true
});
builder.Services.AddKeyedSingleton<IConnectionFactory>("Transaction", new ConnectionFactory
{
    HostName = transactionRabbitConfig.adress,
    Port = transactionRabbitConfig.port,
    UserName = "guest",
    Password = "guest",
    AutomaticRecoveryEnabled = true
});


// Add services to the container.
while (true)
{
    try
    {
        builder.Services.AddSingleton<PublisherServiceBase, CatalogRequestPublisher>();
        builder.Services.AddHostedService<ReplyService>();
        builder.Services.AddSingleton<Publisher2Service>();
        builder.Services.AddHostedService<Reply2Service>();
        builder.Services.AddHostedService<CatalogRequestHandler>();
        break;
    }
    catch (Exception e)
    {
        logger.Error(e, "Failed to connect to RabbitMQ. Retrying in 1 seconds.");
        Thread.Sleep(1000);
    }
}

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
