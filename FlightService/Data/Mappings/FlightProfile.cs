using System.Linq;
using AutoMapper;
using FlightService.DTOs;
using FlightService.Models;

namespace FlightService.Data.Mappings
{
    public class FlightProfile : Profile
    {
        public FlightProfile()
        {
            CreateMap<Flight, FlightSummaryDTO>();

            CreateMap<Flight, FlightDetailsDTO>()
                .ForMember(
                    dest => dest.FlightNumberCodeShares,
                    opt => opt.MapFrom(
                        src => src.FlightNumberCodeShares.Select(
                            fncs => fncs.CodeShare
                        )
                    )
                );

            CreateMap<FlightUpsertDTO, Flight>()
                .ForMember(
                    dest => dest.FlightNumberCodeShares,
                    opt => opt.MapFrom(
                        src => src.FlightNumberCodeShares.Select(
                            codeShare => new FlightNumberCodeShare { CodeShare = codeShare }
                        )
                    )
                );

            CreateMap<FlightDetailsDTO, FlightPublishDTO>();
        }
    }
}