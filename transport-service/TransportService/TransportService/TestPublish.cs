﻿using MessagePack;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransportService.Entities;

namespace TransportService.TransportService
{
    public class TestPublish : BackgroundService
    {
        private readonly PublisherServiceBase transportPublisherTest;
        public TestPublish(PublisherServiceBase test) { 
            transportPublisherTest = test;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //for (int i = 0; i < 10; i++)
            //{

            //}
            int i = 0;
            while (!stoppingToken.IsCancellationRequested) {
                await Task.Delay(1000);
                Console.WriteLine($"Sending message");
                var transport = new Transport() { Id = i.ToString(), Origin = "Polska", Destination = "Niemcy", PricePerTicket = 3.21M, SeatsNumber = 10, SeatsTaken = 1, Type = "Plane", DepartureDate = DateTime.Now, ArrivalDate = DateTime.Now.AddDays(2) };

                var msgId = transportPublisherTest.PublishRequestWithReply<Transport>("resources/transport", "query", MessageType.ADD, transport);

                var body = MessagePackSerializer.Deserialize<Transport>(await transportPublisherTest.GetReply(msgId));

                Console.WriteLine($"Received Reply {body.Id}");
                i++;
            }
            return;  
        }
    }
}