using AutoMapper;
using BookingService.DTOs;
using BookingService.Models;

namespace BookingService.Data.Mappings
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            CreateMap<Booking, GrpcBookingModel>();       
            CreateMap<Booking, BookingSummaryDTO>();
            CreateMap<Booking, BookingDetailsDTO>()
                .ForMember(
                    dest => dest.TotalPrice, 
                    opt => opt.MapFrom(
                        src => src.NumberOfPassengers * src.Flight.Price
                    )
                );
            CreateMap<BookingUpsertDTO, Booking>();
 
            CreateMap<BookingDetailsDTO, BookingPublishDTO>();

            CreateMap<FlightPublishDTO, Flight>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
            CreateMap<FlightDetailsDTO, Flight>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
        }
    }
}