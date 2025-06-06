using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Models.Domains;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using VehicleServiceBook.Middleware; // Add this using for CustomValidationException

namespace VehicleServiceBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IServiceCenterRepository _serviceCenterRepository;
        private readonly IMapper _mapper;

        public AccountController(IUserRepository userRepo, IServiceCenterRepository scRepo, IMapper mapper)
        {
            _userRepository = userRepo;
            _serviceCenterRepository = scRepo;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccountDto dto)
        {
            // 1. Model State Validation (remains here, as it's input validation)
            if (!ModelState.IsValid)
            {
                // Returning BadRequest(ModelState) here is standard for validation errors.
                // This doesn't go through the ExceptionMiddleware directly, but is a 400 Bad Request.
                return BadRequest(ModelState);
            }

            // 2. Business Rule Validation (before database interaction)
            if (dto.Role != "User" && dto.Role != "ServiceCenter")
            {
                // For simple business rule violations like this, you can return BadRequest.
                // Alternatively, you could throw a CustomValidationException here too
                // if you prefer all "bad request" errors to flow through the middleware.
                // For instance: throw new CustomValidationException("Invalid role", 400);
                return BadRequest("Invalid role");
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

            // No try-catch block here for DbUpdateException!
            // We expect _userRepository.SaveChangeAsync() to throw CustomValidationException
            // if a unique key violation occurs, which will then be caught by the global middleware.
            await _userRepository.AddUserAsync(registration);
            await _userRepository.SaveChangeAsync(); // This is where the CustomValidationException will be thrown if email is duplicate

            // If it's a service center, add ServiceCenter details too
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
                // Assume SaveChangesAsync on serviceCenterRepository also handles potential
                // unique constraints for service centers and throws CustomValidationException if needed.
                await _serviceCenterRepository.SaveChangesAsync();
            }

            // If execution reaches here, everything was successful.
            return Ok("Registration successful");
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyAccount()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                // It's good to return NotFound for resource not found.
                // Alternatively, you could throw a specific NotFoundException (a custom exception)
                // and let the middleware handle it for a consistent API response structure,
                // but for simple NotFound cases, returning NotFound() is perfectly acceptable.
                return NotFound("User not found");
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }
    }
}