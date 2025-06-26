using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Models.Domains;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VehicleServiceBook.Middleware;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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

        public AccountController(VehicleServiceBookContext context, IUserRepository userRepo, IServiceCenterRepository scRepo, IMapper mapper)
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

            // Conditional validation for ServiceCenter role
            if (dto.Role == "ServiceCenter")
            {
                if (string.IsNullOrWhiteSpace(dto.ServiceCenterName))
                {
                    ModelState.AddModelError("ServiceCenterName", "Service Center Name is required for ServiceCenter role.");
                }
                if (string.IsNullOrWhiteSpace(dto.ServiceCenterLocation))
                {
                    ModelState.AddModelError("ServiceCenterLocation", "Service Center Location is required for ServiceCenter role.");
                }
                if (string.IsNullOrWhiteSpace(dto.ServiceCenterContact))
                {
                    ModelState.AddModelError("ServiceCenterContact", "Service Center Contact is required for ServiceCenter role.");
                }
                else if (!Regex.IsMatch(dto.ServiceCenterContact, @"^\d{10}$"))
                {
                    ModelState.AddModelError("ServiceCenterContact", "ServiceCenterContact must be exactly 10 digits.");
                }
            }

            // Check ModelState again after conditional validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            try
            {
                await _userRepository.AddUserAsync(registration);
                await _userRepository.SaveChangeAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key"))
                {
                    // More specific check for unique constraint violation, e.g., for email
                    if (_userRepository.GetUserByEmailAsync(dto.Email).Result != null)
                    {
                        return Conflict("Email already registered.");
                    }
                    // Handle other unique constraints if applicable
                }
                // Fallback for other database update exceptions
                return StatusCode(500, "An error occurred while saving the user.");
            }


            if (dto.Role == "ServiceCenter")
            {
                var serviceCenter = new ServiceCenter
                {
                    UserId = registration.UserId,
                    ServiceCenterName = dto.ServiceCenterName,
                    ServiceCenterLocation = dto.ServiceCenterLocation,
                    ServiceCenterContact = dto.ServiceCenterContact
                };

                try
                {
                    await _serviceCenterRepository.AddAsync(serviceCenter);
                    await _serviceCenterRepository.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    // Handle potential unique constraint for service center contact if applicable
                    if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key"))
                    {
                        return Conflict("Service Center Contact already registered.");
                    }
                    return StatusCode(500, "An error occurred while saving service center details.");
                }
            }

            return Ok(new { message = "Registration successful" });
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