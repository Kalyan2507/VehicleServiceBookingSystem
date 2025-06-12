using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Models.Exceptions;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Services;

namespace VehicleServiceBook.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        {
            _service = service;
        }
        private async Task<int?> GetUserIdFromToken()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepo.GetUserByEmailAsync(email);
            return user?.UserId;
        }

        private string GetEmail() => User.FindFirstValue(ClaimTypes.Email);

        [HttpGet]
        public async Task<IActionResult> GetBookings() =>
            Ok(await _service.GetAllBookingsAsync(GetEmail()));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
        }

            return Ok(_mapper.Map<BookingDto>(booking));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDto dto)
        {
            var result = await _service.CreateBookingAsync(dto, GetEmail());
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateBookingDto dto)
        {
            var success = await _service.UpdateBookingAsync(id, dto);
            return success ? Ok("Updated") : NotFound();
        }

        [HttpPut("{id}/update-status-user")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateBookingStatusDto dto)
        {
            var success = await _service.UpdateStatusAsync(id, dto.Status, GetEmail());
            return success ? NoContent() : Forbid();
        }
    }
}