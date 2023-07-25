using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using BookingService.Data.Repos;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace BookingService.Communication.Grpc
{
    public class BookingGrpcService : BookingGrpc.BookingGrpcBase
    {
        private readonly IBookingRepo _repo;
        private readonly IMapper _mapper;

        private readonly ILogger<BookingGrpcService> _logger;

        public BookingGrpcService(IBookingRepo repo, IMapper mapper, ILogger<BookingGrpcService> logger)
        {
            _repo = repo;
            _mapper = mapper;

            _logger = logger;
        }

        public override Task<GetAllBookingsResponse> GetAllBookings(GetAllBookingsRequest request, ServerCallContext ctx)
        {
            var response = new GetAllBookingsResponse();
            var bookings = _repo.GetAllBookings();

            foreach(var booking in bookings)
            {
                response.Bookings.Add(_mapper.Map<GrpcBookingModel>(booking));
            }

            return Task.FromResult(response);
        }
    }
}