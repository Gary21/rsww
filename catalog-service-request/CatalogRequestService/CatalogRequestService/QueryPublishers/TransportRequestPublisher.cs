using CatalogRequestService.DTOs;
using MessagePack;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using RabbitUtilities;
using RabbitUtilities.Configuration;
using ILogger = Serilog.ILogger;

namespace CatalogRequestService.QueryPublishers
{
    public class TransportRequestPublisher : PublisherServiceBase
    {
        private readonly string _exchangeName = "Transports";
        private readonly string _routingKey = "Transports";

        public TransportRequestPublisher(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory)
            : base(logger, connectionFactory, config.GetSection("transportRequestPublisher").Get<ServiceConfig>()!)
        { }


        public async Task<ActionResult> ReserveTransportSeat(TransportReservationDTO transportReservationDTO)
        {
            Guid guid = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.RESERVE, transportReservationDTO);

            CancellationToken cancellationToken = new CancellationToken(false);

            var responseBytes = await GetReply(guid, cancellationToken);

            var response = MessagePackSerializer.Deserialize<ActionResult>(responseBytes);

            return response;
        }

        public async Task<ActionResult> CancelTransportSeat(TransportReservationDTO transportReservationDTO)
        {
            Guid guid = PublishRequestWithReply(_exchangeName, _routingKey, MessageType.DELETE, transportReservationDTO);

            CancellationToken cancellationToken = new CancellationToken(false);

            var responseBytes = await GetReply(guid, cancellationToken);

            var response = MessagePackSerializer.Deserialize<ActionResult>(responseBytes);

            return response;
        }

    }
}
