using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportRequestService.Events
{
    [MessagePackObject]
    public class HotelReservationEvent
    {
        string roomType;

        string destinationCity;
        string destinationCountry;

        int people;
        decimal price;
    }


    [MessagePackObject]
    public class TransportReservationEvent
    {
        string transportType;

        string destinationCity;
        string destinationCountry;
        
        int seats;
        decimal price;
    }

    [MessagePackObject]
    public class Event
    {
        [Key(0)]
        string type;


    }
}
