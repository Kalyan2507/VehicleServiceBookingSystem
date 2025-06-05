using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Models.DTOS;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VehicleServiceBook.Models.Domains;
using Microsoft.EntityFrameworkCore;

namespace VehicleServiceBook.Controllers
{
    [Authorize(Roles = "ServiceCenter")]
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceCenterController : ControllerBase
    {
        private readonly IServiceCenterRepository _serviceCenterRepo;
        private readonly IUserRepository _userRepository;
        private readonly VehicleServiceBookContext _context;

        public ServiceCenterController(
            IServiceCenterRepository serviceCenterRepo,
            IUserRepository userRepository,
            VehicleServiceBookContext context)
        {
            _serviceCenterRepo = serviceCenterRepo;
            _userRepository = userRepository;
            _context = context;
        }

        [HttpPut("update-booking-status/{bookingId}")]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, [FromBody] UpdateBookingStatusDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.Role != "ServiceCenter")
                return Unauthorized();

            var serviceCenter = await _serviceCenterRepo.GetByUserIdAsync(user.UserId);
            if (serviceCenter == null)
                return NotFound("Service center not found.");

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Bookingid == bookingId);
            if (booking == null)
                return NotFound("Booking not found.");

            if (booking.ServiceCenterId != serviceCenter.ServiceCenterId)
                return Forbid("This booking does not belong to your service center.");

            booking.Status = dto.Status;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize(Roles = "ServiceCenter")]
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.Role != "ServiceCenter")
                return Unauthorized();

            var serviceCenter = await _serviceCenterRepo.GetByUserIdAsync(user.UserId);
            if (serviceCenter == null)
                return NotFound("Service Center not found");

            var appointments = await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Registration)
                .Include(b => b.ServiceType)
                .Include(b => b.ServiceCenter)
                .Include(b => b.Invoice)
                .Where(b => b.ServiceCenterId == serviceCenter.ServiceCenterId)
                .Select(b => new AppointmentDto
                {
                    RegistrationNumber = b.Vehicle.RegistrationNumber,
                    CustomerName = b.Registration.Name,
                    Description = b.ServiceType.Description,
                    Date = (DateTime)b.Date,
                    TimeSlot = b.TimeSlot,
                    Price = (decimal)b.ServiceType.Price,
                    Status = b.Status ?? "Pending",
                    PaymentStatus = b.Invoice != null ? b.Invoice.PaymentStatus : "Pending"
                })
                .ToListAsync();

            return Ok(appointments);
        }
        [Authorize(Roles = "ServiceCenter")]
        [HttpPut("update-payment-status/{invoiceId}")]
        public async Task<IActionResult> UpdatePaymentStatus(int invoiceId, [FromBody] UpdatePaymentStatusDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.Role != "ServiceCenter")
                return Unauthorized();

            var serviceCenter = await _serviceCenterRepo.GetByUserIdAsync(user.UserId);
            if (serviceCenter == null)
                return NotFound("Service center not found");

            var invoice = await _context.Invoices
                .Include(i => i.Booking)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice == null)
                return NotFound("Invoice not found");

            if (invoice.Booking.ServiceCenterId != serviceCenter.ServiceCenterId)
                return Forbid("You are not authorized to update this invoice");

            invoice.PaymentStatus = dto.PaymentStatus;
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
