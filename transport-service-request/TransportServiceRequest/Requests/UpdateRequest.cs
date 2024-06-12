using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportRequestService.Requests
{
    [MessagePackObject]
    public class UpdateRequest
    {
        [Key(0)]
        public int Id;
        [Key(1)]
        public int SeatsChange;
        [Key(2)]
        public decimal PriceChange;
    }
}
