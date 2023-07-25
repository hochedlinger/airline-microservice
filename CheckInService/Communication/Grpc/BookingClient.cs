using System;
using System.Collections.Generic;
using AutoMapper;
using BookingService;
using CheckInService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CheckInService.Communication.Grpc
{
    public class BookingClient : IBookingClient
    {
        private readonly IConfiguration _conf;
        private readonly IMapper _mapper;
        
        private readonly ILogger<BookingClient> _logger;

        public BookingClient(IConfiguration conf, IMapper mapper, ILogger<BookingClient> logger)
        {
            _conf = conf;
            _mapper = mapper;

            _logger = logger;
        }

        public IEnumerable<Booking> GetAllBookings()
        {
            var channel = GrpcChannel.ForAddress(_conf["BookingService"]);
            var client = new BookingGrpc.BookingGrpcClient(channel);
            var request = new GetAllBookingsRequest();

            var response = client.GetAllBookings(request);
            return _mapper.Map<IEnumerable<Booking>>(response.Bookings);
        }
    }
}