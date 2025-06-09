using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using Microsoft.EntityFrameworkCore;

namespace VehicleServiceBook.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly VehicleServiceBookContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IServiceCenterRepository _serviceCenterRepository;
        private readonly IInvoiceRepository _repo;
        private readonly IMapper _mapper;

        public InvoiceController(VehicleServiceBookContext context, IUserRepository userRepository, IServiceCenterRepository serviceCenterRepository, IInvoiceRepository repo, IMapper mapper)
        {
            _context = context;
            _userRepository = userRepository;
            _serviceCenterRepository = serviceCenterRepository;
            _repo = repo;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _repo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<InvoiceDto>>(invoices));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var invoice = await _repo.GetByIdAsync(id);
            if (invoice == null)
                return NotFound($"Invoice with ID {id} not found.");

            return Ok(_mapper.Map<InvoiceDto>(invoice));
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return Unauthorized();

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.Bookingid == dto.BookingId && b.UserId == user.UserId);

            if (booking == null)
                return Forbid("You are not authorized to create an invoice for this booking.");

            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(st => st.ServiceTypeId == booking.ServiceTypeId);

            if (serviceType == null)
                return BadRequest("Invalid ServiceTypeId.");

            var invoice = new Invoice
            {
                BookingId = dto.BookingId,
                ServiceTypeId = booking.ServiceTypeId,
                TotalAmount = (double?)serviceType.Price,
                PaymentStatus = "Paid"
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                InvoiceId = invoice.InvoiceId,
                BookingId = invoice.BookingId,
                TotalAmount = invoice.TotalAmount,
                PaymentStatus = invoice.PaymentStatus
            });
        }
    }
}
