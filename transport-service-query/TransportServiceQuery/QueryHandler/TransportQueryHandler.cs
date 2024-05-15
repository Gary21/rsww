using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitUtilities;
using Serilog;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using TransportQueryService.Entities;
using TransportQueryService.Filters;
using TransportQueryService.Queries;
using TransportQueryService.Repositories;


namespace TransportQueryService.QueryHandler
{
    public class TransportQueryHandler : ConsumerServiceBase
    {
        private readonly IDbContextFactory<PostgresRepository> _repositoryFactory;
        public TransportQueryHandler(ILogger logger, IConfiguration config, IConnectionFactory connectionFactory, IDbContextFactory<PostgresRepository> repositoryFactory) 
            : base(logger, connectionFactory, config.GetSection("transportQueryConsumer").Get<RabbitUtilities.Configuration.ConsumerConfig>()!)
        {
            _repositoryFactory = repositoryFactory;
        }

        protected override void ConsumeMessage(object model, BasicDeliverEventArgs ea)
        {
            var headers = ea.BasicProperties.Headers;
            
            if (!headers.TryGetValue("Type", out object? typeObj))
                return;
            var type = (MessageType)Enum.Parse(typeof(MessageType), ASCIIEncoding.ASCII.GetString((byte[])typeObj));

            if (!headers.TryGetValue("Date", out object? dateObj))
                return;
            DateTime.TryParse(ASCIIEncoding.ASCII.GetString((byte[])dateObj), out var date);

            switch (type)
            {
                case MessageType.GET:
                    Get(ea);
                    break;
                default:
                    _logger.Information($"Received message with unknown type.");
                    break;
            }
        }

        private async void Get(BasicDeliverEventArgs ea)
        {
            using var repository = _repositoryFactory.CreateDbContext();
            var message = MessagePackSerializer.Deserialize<TransportGetQuery>(ea.Body.ToArray());
            var filt_ser = MessagePackSerializer.ConvertToJson(MessagePackSerializer.Serialize(message.filters));
            _logger.Information($"GET Transport {filt_ser}");
            //message.sort?

            //message.filters
            var transports = await repository.Transports
                .Where(t => message.filters.Ids == null || message.filters.Ids.Count() == 0 || message.filters.Ids.Contains(t.Id))
                .Where(t => message.filters.Types == null || message.filters.Types.Count() == 0 || message.filters.Types.Contains(t.Type))
                .Where(t => message.filters.DepartureDates == null || message.filters.DepartureDates.Count() == 0 || message.filters.DepartureDates.Contains(t.DepartureDate))
                .Where(t => message.filters.ArrivalDates == null || message.filters.ArrivalDates.Count() == 0 || message.filters.ArrivalDates.Contains(t.ArrivalDate))
                .Where(t => message.filters.CountryDestinations == null || message.filters.CountryDestinations.Count() == 0 || message.filters.CountryDestinations.Contains(t.DestinationCountry))
                .Where(t => message.filters.CountryOrigins == null || message.filters.CountryOrigins.Count() == 0 || message.filters.CountryOrigins.Contains(t.OriginCountry))
                .Where(t => message.filters.CityDestinations == null || message.filters.CityDestinations.Count() == 0 || message.filters.CityDestinations.Contains(t.DestinationCity))
                .Where(t => message.filters.CityOrigins == null || message.filters.CityOrigins.Count() == 0 || message.filters.CityOrigins.Contains(t.OriginCity))
                //.Where(t => t.SeatsNumber - t.SeatsTaken >= message.filters.AvailableSeats || message.filters.AvailableSeats == null)
                .ToListAsync();
            var transportEvents = await repository.TransportEvents
                .Where(t => transports.Select(x=>x.Id).Contains(t.TransportId))
                .OrderBy(t => t.SequenceNumber)
                .ToListAsync();

            foreach (var transport in transports)
            {
                repository.Entry(transport).State = EntityState.Detached;
                foreach (var transportEvent in transportEvents.Where(x=>x.TransportId==transport.Id))
                {
                    transport.SeatsTaken += transportEvent.SeatsChange;
                    transport.PricePerTicket += transportEvent.PriceChange;
                }
            }
            transports = transports.Where(t => t.SeatsNumber - t.SeatsTaken >= message.filters.AvailableSeats || message.filters.AvailableSeats == null).ToList();

            var serialized = MessagePackSerializer.Serialize(transports);
            Reply(ea, serialized);
        }

    }
}
