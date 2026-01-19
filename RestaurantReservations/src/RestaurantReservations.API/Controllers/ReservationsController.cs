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

        // GET: api/reservations/status
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { status = "API is running", timestamp = DateTime.UtcNow });
        }

        // GET: api/reservations
        [HttpGet]
        public async Task<IActionResult> GetAllReservations([FromQuery] DateTime? date)
        {
            if (date.HasValue)
            {
                var reservations = await _reservationService.GetReservationsByDateAsync(date.Value);
                return Ok(reservations);
            }

            var allReservations = await _reservationService.GetAllReservationsAsync();
            return Ok(allReservations);
        }

        // GET: api/reservations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReservation(int id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound($"Reserva com ID {id} não encontrada.");
            }

            return Ok(reservation);
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdReservation = await _reservationService.CreateReservationAsync(dto);
                return CreatedAtAction(nameof(GetReservation), new { id = createdReservation.Id }, createdReservation);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        // PUT: api/reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] UpdateReservationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedReservation = await _reservationService.UpdateReservationAsync(id, dto);
                if (updatedReservation == null)
                {
                    return NotFound($"Reserva com ID {id} não encontrada.");
                }

                return Ok(updatedReservation);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { error = ex.Message });
            }
        }

        // DELETE: api/reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var result = await _reservationService.DeleteReservationAsync(id);
            if (!result)
            {
                return NotFound($"Reserva com ID {id} não encontrada.");
            }

            return NoContent();
        }
    }
}