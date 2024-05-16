using CatalogRequestService.DTOs;
using NuGet.Protocol.Core.Types;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using ILogger = Serilog.ILogger;
using MessagePack;
using CatalogQueryService.DTOs;
using Microsoft.AspNetCore.Mvc;



namespace CatalogRequestService.QueryPublishers
{
    public class HotelRequestPublisher : PublisherServiceBase
    {
        private readonly string _exchangeName = "Hotels";
        private readonly string _routingKey = "Hotels";

        public HotelRequestPublisher(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory)
            : base(logger, connectionFactory, config.GetSection("hotelRequestPublisher").Get<ServiceConfig>()!)
        {}

        //public async Task<ICollection<HotelDTO>> GetHotelsAll()
        //{
        //    Guid messageCorellationId = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.GET, "Hotels/all");

        //    CancellationToken cancellationToken = new CancellationToken(false);

        //    var hotelsBytes = await GetReply(messageCorellationId, cancellationToken);

        //    var hotels = MessagePackSerializer.Deserialize<ICollection<HotelDTO>>(hotelsBytes);

        //    return hotels;
        //}

        public async Task<ActionResult> ReserveHotelRoom(HotelReservationDTO hotelReservationDTO)
        {
            Guid guid = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.RESERVE, hotelReservationDTO);

            CancellationToken cancellationToken = new CancellationToken(false);

            var responseBytes = await GetReply(guid, cancellationToken);

            var response = MessagePackSerializer.Deserialize<ActionResult>(responseBytes);

            return response;
        }

        public async Task<ActionResult> CancelHotelRoom(HotelReservationDTO hotelReservationDTO)
        {
            Guid guid = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.DELETE, hotelReservationDTO);

            CancellationToken cancellationToken = new CancellationToken(false);

            var responseBytes = await GetReply(guid, cancellationToken);

            var response = MessagePackSerializer.Deserialize<ActionResult>(responseBytes);

            return response;
        }



    }
}
