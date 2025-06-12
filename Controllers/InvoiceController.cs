using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Services;

namespace VehicleServiceBook.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _service;

        public InvoiceController(IInvoiceService service)

        {

            _service = service;

        }

        private string GetEmail() => User.FindFirstValue(ClaimTypes.Email);

        [HttpGet]

        public async Task<IActionResult> GetAll()

        {

            var result = await _service.GetAllAsync(GetEmail());

            return Ok(result);

        }

        [HttpGet("{id}")]

        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetById(int id)

        {

            var result = await _service.GetByIdAsync(id, GetEmail());

            return result == null ? NotFound("Invoice not found or access denied") : Ok(result);

        }

        [HttpPost]

        [Authorize(Roles = "User")]

        public async Task<IActionResult> Create(CreateInvoiceDto dto)

        {

            var email = User.FindFirstValue(ClaimTypes.Email);

            var result = await _service.CreateAsync(dto, email);

            if (result is string message)

            {

                if (message == "Already paid")

                    return BadRequest(new { error = message });

                return BadRequest(new { error = message });

            }

            if (result == null)

                return Forbid("Unauthorized or invalid booking.");

            return Ok(result);

        }

    }
}
