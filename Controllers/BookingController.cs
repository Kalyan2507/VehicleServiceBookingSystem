using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly VehicleServiceBookContext _context;
        private readonly IBookingRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public BookingController(VehicleServiceBookContext context, IBookingRepository repo, IUserRepository userRepo, IMapper mapper)
        {
            _context = context;
            _repo = repo;
            _userRepo = userRepo;
            _mapper = mapper;
        }
        private async Task<int?> GetUserIdFromToken()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepo.GetUserByEmailAsync(email);
            return user?.UserId;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAll()
        //{
        //    var userId = await GetUserIdFromToken();
        //    var bookings = await _repo.GetAllByUserIdAsync(userId.Value);
        //    return Ok(_mapper.Map<IEnumerable<BookingDto>>(bookings));
        //}
        //[HttpGet("my-bookings")]
        //public async Task<IActionResult> GetBookingsForLoggedInUser()
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    var user = await _userRepo.GetUserByEmailAsync(email);
        //    if (user == null || user.Role != "User")
        //        return Unauthorized("You are not authorized.");

        //    var bookings = await _repo.GetAllByUserIdAsync(user.UserId);
        //    var dtoList = _mapper.Map<IEnumerable<BookingDto>>(bookings);

        //    return Ok(dtoList);
        //}
        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepo.GetUserByEmailAsync(email);

            var bookings = await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.ServiceType)
                .Include(b => b.Mechanic)
                .Include(b => b.ServiceCenter)
                .Where(b => b.UserId == user.UserId)
                .Select(b => new BookingDto
                {
                    BookingId = b.Bookingid,
                    RegistrationNumber = b.Vehicle.RegistrationNumber,
                    VehicleId = b.Vehicle.VehicleId,
                    Description = b.ServiceType.Description,
                    MechanicName = b.Mechanic != null ? b.Mechanic.MechanicName : "Not Assigned",
                    Date = b.Date,
                    TimeSlot = b.TimeSlot,
                    Status = b.Status,
                    ServiceTypeId = b.ServiceTypeId,
                    ServiceCenterId = b.ServiceCenterId
                })
                .ToListAsync();

            return Ok(bookings);
        }

        
        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepo.GetUserByEmailAsync(email);

            var booking = await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.ServiceType)
                .Include(b => b.Mechanic)
                .Include(b => b.ServiceCenterId)
                .Include(b => b.ServiceTypeId)
                .FirstOrDefaultAsync(b => b.Bookingid == id && b.UserId == user.UserId);

            if (booking == null)
                return NotFound("Booking not found.");

            var dto = new BookingDto
            {
                BookingId = booking.Bookingid,
                RegistrationNumber = booking.Vehicle?.RegistrationNumber,
                VehicleId = (int)(booking.Vehicle?.VehicleId),
                Description = booking.ServiceType?.Description,
                Date = booking.Date,
                TimeSlot = booking.TimeSlot,
                Status = booking.Status,
                MechanicName = booking.Mechanic != null ? booking.Mechanic.MechanicName : "Not Assigned",
                ServiceTypeId = booking.ServiceTypeId,
                ServiceCenterId = booking.ServiceCenterId
            };

            return Ok(dto);
        }


        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepo.GetUserByEmailAsync(email);

            if (user == null)
                return Unauthorized();

            // Step 1: Find any available mechanic in that service center
            var mechanic = await _context.Mechanics
                .FirstOrDefaultAsync(m => m.ServiceCenterId == dto.ServiceCenterId);

            int? assignedMechanicId = mechanic?.Mechanicid;

            var booking = new Booking
            {
                UserId = user.UserId,
                VehicleId = dto.VehicleId,
                ServiceCenterId = dto.ServiceCenterId,
                ServiceTypeId = (int)dto.ServiceTypeId,
                Date = dto.Date,
                TimeSlot = dto.TimeSlot,
                Status = "Pending",
                MechanicId = assignedMechanicId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            if (mechanic == null)
                return BadRequest("No Mechanic available in the selected service center");
            return Ok(_mapper.Map<BookingDto>(booking));

            //return Ok(booking.Bookingid);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id);
            if (booking == null)
                return NotFound($"Booking with ID {id} not found");

            booking.VehicleId = dto.VehicleId;
            booking.ServiceCenterId = dto.ServiceCenterId;
            booking.ServiceTypeId = (int)dto.ServiceTypeId;
            booking.Date = dto.Date;
            booking.TimeSlot = dto.TimeSlot;
            //booking.Status = dto.Status;

            await _repo.UpdateAsync(booking);
            await _repo.SaveChangesAsync();

            return Ok("Booking Updated Successfully");
        }
        [Authorize(Roles = "User")]
        [HttpPut("{id}/update-status-user")]
        public async Task<IActionResult> UpdateStatusByUser(int id, [FromBody] UpdateBookingStatusDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null || user.Role != "User")
                return Unauthorized();

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Bookingid == id);
            if (booking == null)
                return NotFound("Booking not found.");

            if (booking.UserId != user.UserId)
                return Forbid("You are not authorized to update this booking.");

            booking.Status = dto.Status;
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    
    }
}