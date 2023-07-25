using System;
using System.Collections.Generic;
using AutoMapper;
using BookingService.Communication;
using BookingService.Data.Repos;
using BookingService.DTOs;
using BookingService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookingService.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepo _repo;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _msgBusClient;

        private readonly ILogger<BookingsController> _logger;

        public BookingsController(IBookingRepo repo, IMapper mapper, IMessageBusClient msgBusClient, ILogger<BookingsController> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _msgBusClient = msgBusClient;

            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<BookingSummaryDTO>> GetAllBookings()
        {
            var bookings = _repo.GetAllBookings();
            var bookingSummaryDTOs = _mapper.Map<IEnumerable<BookingSummaryDTO>>(bookings);
            return Ok(bookingSummaryDTOs);
        }

        [HttpGet("flight/{flightId}")]
        public ActionResult<IEnumerable<BookingSummaryDTO>> GetBookingsByFlightId(int flightId)
        {
            var bookings = _repo.GetBookingsByFlightId(flightId);
            var bookingSummaryDTOs = _mapper.Map<IEnumerable<BookingSummaryDTO>>(bookings);
            return Ok(bookingSummaryDTOs);
        }

        [HttpGet("{id}", Name = "GetBookingById")]
        public ActionResult<BookingDetailsDTO> GetBookingById(int id)
        {
            var booking = _repo.GetBookingById(id);
            if (booking == null) { return NotFound(); }

            var bookingDetailsDTO = _mapper.Map<BookingDetailsDTO>(booking);
            return Ok(bookingDetailsDTO);
        }

        [HttpPost]
        public ActionResult<BookingDetailsDTO> CreateBooking(BookingUpsertDTO bookingCreateDTO)
        {
            if (!_repo.FlightExist(bookingCreateDTO.FlightId)) {
                return BadRequest($"No flight with flight id {bookingCreateDTO.FlightId} was found");
            }

            if (bookingCreateDTO.NumberOfPassengers < 1 || bookingCreateDTO.NumberOfPassengers > 10) {
                return BadRequest("The number of passengers per booking must be between 1 and 10.");
            }

            var booking = _mapper.Map<Booking>(bookingCreateDTO);
            booking.BookingDate = DateTime.Now;

            try {    
                _repo.CreateBooking(booking);
                _repo.SaveChanges();
            }
            catch (InvalidOperationException e) {
                return Conflict(e.Message);
            }

            var bookingDetailsDTO = _mapper.Map<BookingDetailsDTO>(booking);

            try {
                var bookingPublishDTO = _mapper.Map<BookingPublishDTO>(bookingDetailsDTO);
                bookingPublishDTO.Event = "BookingCreated"; 

                _msgBusClient.Publish(bookingPublishDTO);
            }
            catch (Exception e) {
                _logger.LogWarning($"Publishing new flight on message bus failed: {e.Message}");
            }

            return CreatedAtRoute(nameof(GetBookingById), new {Id = bookingDetailsDTO.Id}, bookingDetailsDTO);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateBooking(int id, BookingUpsertDTO bookingUpdateDTO)
        {
            var booking = _repo.GetBookingById(id);
            if (booking == null) { return NotFound(); }

            if (!_repo.FlightExist(bookingUpdateDTO.FlightId)) {
                return BadRequest($"No flight with flight id {bookingUpdateDTO.FlightId} was found");
            }

            if (bookingUpdateDTO.NumberOfPassengers < 1 || bookingUpdateDTO.NumberOfPassengers > 10) {
                return BadRequest("The number of passengers per booking must be between 1 and 10.");
            }

            _mapper.Map(bookingUpdateDTO, booking);
            booking.BookingDate = DateTime.Now;
            
            try {    
                _repo.UpdateBooking(booking);
                _repo.SaveChanges();
            }
            catch (InvalidOperationException e) {
                return Conflict(e.Message);
            }

            var bookingDetailsDTO = _mapper.Map<BookingDetailsDTO>(booking);

            try {
                var bookingPublishDTO = _mapper.Map<BookingPublishDTO>(bookingDetailsDTO);
                bookingPublishDTO.Event = "BookingUpdated"; 

                _msgBusClient.Publish(bookingPublishDTO);
            }
            catch (Exception e) {
                _logger.LogWarning($"Publishing new flight on message bus failed: {e.Message}");
            }

            return NoContent();
        }
    }
}