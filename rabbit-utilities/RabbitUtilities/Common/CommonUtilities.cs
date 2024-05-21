using Microsoft.Extensions.Hosting.Internal;
using RabbitMQ.Client;
using Serilog;
namespace RabbitUtilities.Common
{
    interface RabbitClient
    {
        protected ILogger _logger {get;}
        public void ConnectToRabbit(IConnectionFactory connectionFactory, CancellationToken cancellationToken, out IConnection? connection)
        {
            connection = null;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    connection = connectionFactory.CreateConnection();
                    _logger.Information($"RabbitMQ connection established");
                    break;
                }
                catch (RabbitMQ.Client.Exceptions.BrokerUnreachableException ex)
                {
                    _logger.Warning($"RabbitMQ unreachable {ex.HResult}, trying again in 3 seconds");
                    Task.Delay(3000).Wait(cancellationToken);
                }
            }
        }

    }

    
    static class CommonUtilities
    {
    }
}
