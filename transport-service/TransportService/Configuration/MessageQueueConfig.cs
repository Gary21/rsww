using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportService.Configuration
{
    public record MessageQueueConfig(string adress, int port, string domain) {
        //public int Port { get { return Int32.Parse(port); } }
        //public string GetConnectionString() { return $"{adress}:{port}"; }
    };

}
