using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Services;

namespace VehicleServiceBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterAccountDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Role != "User" && dto.Role != "ServiceCenter")
                return BadRequest("Invalid role");

            var success = await _userService.RegisterAccountAsync(dto);

            if (!success)
                return BadRequest("User Already Exists, Please Login");

            return Ok("Registration successful");
        }

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetMe()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var profile = await _userService.GetProfileAsync(email);

            if (profile == null)
                return Unauthorized();

            return Ok(profile);
        }

        [Authorize]
        [HttpPut("Profile")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateAccountDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var result = await _userService.UpdateProfileAsync(email, dto);

            if (!result)
                return NotFound("User or Service center data not found.");

            return Ok();
        }
    }
}