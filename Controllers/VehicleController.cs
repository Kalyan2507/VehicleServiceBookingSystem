using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Services;

namespace VehicleServiceBook.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleService _service;

        public VehicleController(IVehicleService service)

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

        public async Task<IActionResult> GetById(int id)

        {

            var vehicle = await _service.GetByIdAsync(id, GetEmail());

            if (vehicle == null)

                return Forbid("You are not authorized to access this vehicle.");

            return Ok(vehicle);

        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateVehicleDto dto)

        {

            var result = await _service.CreateAsync(dto, GetEmail());

            return Ok(result);

        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, CreateVehicleDto dto)

        {

            var message = await _service.UpdateAsync(id, dto, GetEmail());

            return message == null ? NotFound("Vehicle not found or access denied.") : Ok(message);

        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)

        {

            var message = await _service.DeleteAsync(id, GetEmail());

            return message == null ? NotFound("Vehicle not found or access denied.") : Ok(message);

        }

    }
}
