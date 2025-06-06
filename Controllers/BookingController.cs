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
    [Authorize(Roles ="User")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly VehicleServiceBookContext _vehicleServiceBookContext;
        private readonly IBookingRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        public BookingController(VehicleServiceBookContext vehicleServiceBookContext, IBookingRepository repo,IUserRepository userRepo,IMapper mapper)
        {
            _vehicleServiceBookContext = vehicleServiceBookContext;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = await GetUserIdFromToken();
            var bookings = await _repo.GetAllByUserIdAsync(userId.Value);
            return Ok(_mapper.Map<IEnumerable<BookingDto>>(bookings));
        }
        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetBookingsForLoggedInUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null || user.Role != "User")
                return Unauthorized("You are not authorized.");

            var bookings = await _repo.GetAllByUserIdAsync(user.UserId);
            var dtoList = _mapper.Map<IEnumerable<BookingDto>>(bookings);

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Unauthorized("data not there");
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null) return Unauthorized();
            var booking = await _vehicleServiceBookContext.Bookings.FirstOrDefaultAsync(b => b.Bookingid == id && b.UserId == user.UserId);
            if (booking == null)
                return NotFound($"Booking with ID {id} not found");
            var dto = _mapper.Map<BookingDto>(booking);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            var userId = await GetUserIdFromToken();
            var booking = _mapper.Map<Booking>(dto);
            booking.UserId = userId.Value;

            await _repo.AddAsync(booking);
            await _repo.SaveChangesAsync();

            return Ok(_mapper.Map<BookingDto>(booking));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id);
            if (booking == null) return NotFound();

            booking.Date = dto.Date;
            booking.TimeSlot = dto.TimeSlot;
            booking.Status = dto.Status;
            booking.ServiceCenterId = dto.ServiceCenterId;
            booking.ServiceTypeId = dto.ServiceTypeId;
            booking.VehicleId = dto.VehicleId;

            await _repo.UpdateAsync(booking);
            await _repo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking=await _repo.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound($"No booking found with ID {id}");
            }
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return NoContent();
        }
    }
}
