using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Models.Domains;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (dto.Role != "User" && dto.Role != "ServiceCenter")
                return BadRequest("Invalid role");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create Registration record
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
                await _serviceCenterRepository.SaveChangesAsync();
            }

            return Ok("Registration successful");
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult>GetMyAccount()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
                return NotFound("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }
    }
}
