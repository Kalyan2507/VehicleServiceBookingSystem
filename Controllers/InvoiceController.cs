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
        [Authorize(Roles = "ServiceCenter")]
        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto dto)
        {
            // 1. Validate Booking exists
            var booking = await _context.Bookings
                .Include(b => b.ServiceCenter)
                .FirstOrDefaultAsync(b => b.Bookingid == dto.BookingId);

            if (booking == null)
                return NotFound("Booking not found");

            // 2. Check ServiceCenter is the owner of this booking
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);
            var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(user.UserId);

            if (booking.ServiceCenterId != serviceCenter.ServiceCenterId)
                return Forbid("You cannot invoice for a booking you don't own");

            // 3. Get the ServiceType to calculate price
            var serviceType = await _context.ServiceTypes.FirstOrDefaultAsync(st => st.ServiceTypeId == dto.ServiceTypeId);
            if (serviceType == null)
                return BadRequest("Invalid service type");

            // 4. Create and save invoice
            var invoice = new Invoice
            {
                BookingId = dto.BookingId,
                ServiceTypeId = dto.ServiceTypeId,
                TotalAmount = (double?)serviceType.Price,
                PaymentStatus = "Pending"
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            var result = _mapper.Map<InvoiceDto>(invoice);

            return Ok(result); // or map to InvoiceDto
        }
    }
}
