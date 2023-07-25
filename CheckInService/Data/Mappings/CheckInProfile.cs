using AutoMapper;
using BookingService;
using CheckInService.DTOs;
using CheckInService.Models;

namespace CheckInService.Data.Mappings
{
    public class CheckInProfile : Profile
    {
        public CheckInProfile()
        {
            CreateMap<CheckIn, CheckInSummaryDTO>();
            CreateMap<CheckIn, CheckInDetailsDTO>();
            CreateMap<CheckInUpsertDTO, CheckIn>(); 
            CreateMap<FlightPublishDTO, Flight>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore());   
            CreateMap<FlightDetailsDTO, Flight>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<BookingPublishDTO, Booking>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))       
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<GrpcBookingModel, Booking>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

        }
    }
}