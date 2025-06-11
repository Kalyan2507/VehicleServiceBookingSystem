using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Models.DTOS;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VehicleServiceBook.Models.Domains;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Services;

namespace VehicleServiceBook.Controllers
{
    [Authorize(Roles = "ServiceCenter")]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceCenterController : ControllerBase
    {
        private readonly IServiceCenterService _service;

        public ServiceCenterController(IServiceCenterService service)

        {

            _service = service;

        }

        private int GetUserId() =>

            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpGet("appointments")]

        public async Task<IActionResult> GetAppointments()

        {

            var result = await _service.GetAppointmentsAsync(GetUserId());

            if (result == null) return NotFound("No appointments found.");

            return Ok(result);

        }

        [HttpPut("update-booking-status/{bookingId}")]

        public async Task<IActionResult> UpdateBookingStatus(int bookingId, [FromBody] UpdateBookingStatusDto dto)

        {

            var success = await _service.UpdateBookingStatusAsync(GetUserId(), bookingId, dto.Status);

            if (!success) return Forbid("You are not authorized or booking not found.");

            return NoContent();

        }

        [HttpPut("assign-mechanic/{bookingId}")]

        public async Task<IActionResult> AssignMechanic(int bookingId, [FromBody] AssignMechanicDto dto)

        {

            var success = await _service.AssignMechanicAsync(GetUserId(), bookingId, dto.MechanicId);

            if (!success) return Forbid("Assignment failed. Check permissions or data.");

            return NoContent();

        }



    }
}
