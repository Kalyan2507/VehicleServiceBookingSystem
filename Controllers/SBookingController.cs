using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SBookingController : ControllerBase
    {
        private readonly ISBookingRepository _bookingRepository;

        public SBookingController(ISBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await _bookingRepository.GetAllBookingsAsync();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingRepository.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();
            return Ok(booking);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBookingStatus(int id, [FromBody] string status)
        {
            var updated = await _bookingRepository.UpdateBookingStatusAsync(id, status);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}
