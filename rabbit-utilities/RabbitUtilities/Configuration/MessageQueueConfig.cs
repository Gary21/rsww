using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitUtilities.Configuration
{
    //public record MessageQueueConfig(string adress, int port, string exchange, string queue, string routing);
    public record RabbitConfig(string adress, int port, string user, string password);
    public record ConsumerConfig(string exchange, string queue, string routing);
    public record SubscriberConfig(string exchange);
    public record ServiceConfig(string name);

}
