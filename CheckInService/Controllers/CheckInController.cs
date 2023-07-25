using System;
using System.Collections.Generic;
using AutoMapper;
using CheckInService.Data.Repos;
using CheckInService.DTOs;
using CheckInService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookingService.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckInController : ControllerBase
    {
        private readonly ICheckInRepo _repo;
        private readonly IMapper _mapper;

        private readonly ILogger<CheckInController> _logger;

        public CheckInController(ICheckInRepo repo, IMapper mapper, ILogger<CheckInController> logger)
        {
            _repo = repo;
            _mapper = mapper;

            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CheckInSummaryDTO>> GetAllCheckIns()
        {
            var checkIns = _repo.GetAllCheckIns();
            var checkInDTOs = _mapper.Map<IEnumerable<CheckInSummaryDTO>>(checkIns);
            return Ok(checkIns);
        }

        [HttpGet("booking/{bookingId}")]
        public ActionResult<IEnumerable<CheckInSummaryDTO>> GetAllCheckInsByBookingId(int bookingId)
        {
            var checkIns = _repo.GetAllCheckInsByBookingId(bookingId);
            var checkInDTOs = _mapper.Map<IEnumerable<CheckInSummaryDTO>>(checkIns);
            return Ok(checkInDTOs);
        }

        [HttpGet("flight/{flightId}")]
        public ActionResult<IEnumerable<CheckInSummaryDTO>> GetAllCheckInsByFlightId(int flightId)
        {
            var checkIns = _repo.GetAllCheckInsByFlightId(flightId);
            var checkInDTOs = _mapper.Map<IEnumerable<CheckInSummaryDTO>>(checkIns);
            return Ok(checkInDTOs);
        }

        [HttpGet("{id}")]
        public ActionResult<CheckInDetailsDTO> GetCheckInById(string id)
        {
            var checkIn = _repo.GetCheckInById(id);
            if (checkIn == null)
            {
                return NotFound();
            }
            var checkInDTO = _mapper.Map<CheckInDetailsDTO>(checkIn);
            return Ok(checkInDTO);
        }

        [HttpPost]
        public ActionResult<CheckInDetailsDTO> CreateCheckIn(CheckInUpsertDTO checkInDTO)
        {
            var checkIn = _mapper.Map<CheckIn>(checkInDTO);
            checkIn.CheckInTime = DateTime.Now;

            try {    
                _repo.CreateCheckIn(checkIn);
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }

            var resultDTO = _mapper.Map<CheckInDetailsDTO>(checkIn);
            return CreatedAtAction(nameof(GetCheckInById), new { id = resultDTO.Id }, resultDTO);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateCheckIn(string id, CheckInUpsertDTO checkInDTO)
        {
            var checkIn = _repo.GetCheckInById(id);
            if (checkIn == null)
            {
                return NotFound();
            }

            try { 
                _mapper.Map(checkInDTO, checkIn);
                _repo.UpdateCheckIn(checkIn);
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }
            return NoContent();
        }
    }
}