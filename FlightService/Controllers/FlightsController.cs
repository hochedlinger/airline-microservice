using System;
using System.Collections.Generic;
using AutoMapper;
using FlightService.Communication;
using FlightService.Data.Repos;
using FlightService.DTOs;
using FlightService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FlightService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightRepo _repo;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _msgBusClient;

        private readonly ILogger<FlightsController> _logger;

        public FlightsController(IFlightRepo repo, IMapper mapper, IMessageBusClient msgBusClient, ILogger<FlightsController> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _msgBusClient = msgBusClient;
            
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FlightSummaryDTO>> GetAllFlights()
        {
            var flights = _repo.GetAllFlights();
            var flightsReadDTO = _mapper.Map<IEnumerable<FlightSummaryDTO>>(flights);
            return Ok(flightsReadDTO);
        }

        [HttpGet("{id}", Name = "GetFlightById")]
        public ActionResult<FlightDetailsDTO> GetFlightById(int id)
        {
            var flight = _repo.GetFlightById(id);
            if (flight == null) { return NotFound(); }

            var flightDetailsDTO = _mapper.Map<FlightDetailsDTO>(flight);
            return Ok(flightDetailsDTO);
        }

        [HttpPost]
        public ActionResult<FlightDetailsDTO> CreateFlight(FlightUpsertDTO flightCreateDTO)
        {
            var flight = _mapper.Map<Flight>(flightCreateDTO);
            _repo.CreateFlight(flight);
            _repo.SaveChanges();

            var flightDetailsDTO = _mapper.Map<FlightDetailsDTO>(flight);

            try {
                var flightPublishDTO = _mapper.Map<FlightPublishDTO>(flightDetailsDTO);
                flightPublishDTO.Event = "FlightCreated"; 

                _msgBusClient.Publish(flightPublishDTO);
            }
            catch (Exception e) {
                _logger.LogWarning($"Publishing new flight on message bus failed: {e.Message}");
            }

            return CreatedAtRoute(nameof(GetFlightById), new {Id = flightDetailsDTO.Id}, flightDetailsDTO);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateFlight(int id, FlightUpsertDTO flightUpdateDTO)
        {
            var flightFromRepo = _repo.GetFlightById(id);
            if (flightFromRepo == null) { return NotFound(); }

            _mapper.Map(flightUpdateDTO, flightFromRepo);
            _repo.UpdateFlight(flightFromRepo);
            _repo.SaveChanges();

            var flightDetailsDTO = _mapper.Map<FlightDetailsDTO>(flightFromRepo);

            try {
                var flightPublishDTO = _mapper.Map<FlightPublishDTO>(flightDetailsDTO);
                flightPublishDTO.Event = "FlightUpdated"; 

                _msgBusClient.Publish(flightPublishDTO);
            }
            catch (Exception e) {
                _logger.LogWarning($"Publishing new flight on message bus failed: {e.Message}");
            }

            return NoContent();
        }

    }
}