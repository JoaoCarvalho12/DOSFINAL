using Microsoft.AspNetCore.Mvc;
using RestaurantReservations.API.DTOs;
using RestaurantReservations.API.Services;

namespace RestaurantReservations.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "API is running", timestamp = DateTime.UtcNow });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDto>>> GetReservations([FromQuery] DateTime? date = null)
        {
            if (date.HasValue)
            {
                var reservations = await _reservationService.GetReservationsByDateAsync(date.Value);
                return Ok(reservations);
            }

            var allReservations = await _reservationService.GetAllReservationsAsync();
            return Ok(allReservations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDto>> GetReservation(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
                return NotFound();
            return Ok(reservation);
        }

        [HttpPost]
        public async Task<ActionResult<ReservationDto>> CreateReservation(CreateReservationDto dto)
        {
            try
            {
                var reservation = await _reservationService.CreateReservationAsync(dto);
                return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReservationDto>> UpdateReservation(int id, UpdateReservationDto dto)
        {
            try
            {
                var updatedReservation = await _reservationService.UpdateReservationAsync(id, dto);
                
                if (updatedReservation == null)
                    return NotFound();
                    
                return Ok(updatedReservation);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var result = await _reservationService.DeleteReservationAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
    }
}