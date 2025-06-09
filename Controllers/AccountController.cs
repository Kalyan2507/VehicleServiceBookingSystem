using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Models.Domains;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VehicleServiceBook.Middleware;
using Microsoft.EntityFrameworkCore; // Add this using for CustomValidationException

namespace VehicleServiceBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly VehicleServiceBookContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IServiceCenterRepository _serviceCenterRepository;
        private readonly IMapper _mapper;

        public AccountController(VehicleServiceBookContext context,IUserRepository userRepo, IServiceCenterRepository scRepo, IMapper mapper)
        {
            _context = context;
            _userRepository = userRepo;
            _serviceCenterRepository = scRepo;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccountDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto.Role != "User" && dto.Role != "ServiceCenter")
                return BadRequest("Invalid role");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var registration = new Registration
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                PasswordHash = hashedPassword,
                Role = dto.Role
            };

            await _userRepository.AddUserAsync(registration);
            await _userRepository.SaveChangeAsync();

            if (dto.Role == "ServiceCenter")
            {
                var serviceCenter = new ServiceCenter
                {
                    UserId = registration.UserId,
                    ServiceCenterName = dto.ServiceCenterName,
                    ServiceCenterLocation = dto.ServiceCenterLocation,
                    ServiceCenterContact = dto.ServiceCenterContact
                };

                await _serviceCenterRepository.AddAsync(serviceCenter);
                await _serviceCenterRepository.SaveChangesAsync();
            }

            return Ok("Registration successful");
        }
        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetMe()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                return Unauthorized();

            if (user.Role == "ServiceCenter")
            {
                var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(user.UserId);
                if (serviceCenter == null)
                    return NotFound("Service center info not found.");

                var dto = new ProfileDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,

                    ServiceCenterId = serviceCenter.ServiceCenterId,
                    ServiceCenterName = serviceCenter.ServiceCenterName,
                    ServiceCenterLocation = serviceCenter.ServiceCenterLocation,
                    ServiceCenterContact = serviceCenter.ServiceCenterContact
                };

                return Ok(dto);
            }

            var dtoUser = _mapper.Map<UserDto>(user);
            return Ok(dtoUser);
        }
        [Authorize]
        [HttpPut("Profile")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateAccountDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                return Unauthorized();

            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            _context.Registrations.Update(user);


            if (user.Role == "ServiceCenter")
            {
                var serviceCenter = await _context.ServiceCenters.FirstOrDefaultAsync(sc => sc.UserId == user.UserId);
                if (serviceCenter == null)
                    return NotFound("Service center data not found.");

                serviceCenter.ServiceCenterName = dto.ServiceCenterName;
                serviceCenter.ServiceCenterLocation = dto.ServiceCenterLocation;
                serviceCenter.ServiceCenterContact = dto.ServiceCenterContact;

                _context.ServiceCenters.Update(serviceCenter);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}