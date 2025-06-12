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
    [Authorize(Roles ="ServiceCenter")]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MechanicController : ControllerBase
    {
        private readonly IMechanicService _service;

        {

            _service = service;

        }
        //[HttpGet]
        //public async Task<IActionResult>GetAll()
        //{
        //    var mechanics=await _repo.GetAllAsync();
        //    return Ok(_mapper.Map<IEnumerable<MechanicDto>>(mechanics));
        //}

        private string GetEmail() => User.FindFirstValue(ClaimTypes.Email);

        [HttpGet("my-mechanics")]

        public async Task<IActionResult> GetByLoggedInServiceCenter()

        {

            var result = await _service.GetMyMechanicsAsync(GetEmail());

            return Ok(result);

        }

        [HttpGet("{id}")]

        public async Task<IActionResult> GetById(int id)

        {
        }

        [HttpPost]

        public async Task<IActionResult> Create(CreateMechanicDto dto)

        {

            var result = await _service.CreateAsync(dto, GetEmail());

            return Ok(result);

        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Update(int id, CreateMechanicDto dto)

        {

            var result = await _service.UpdateAsync(id, dto, GetEmail());

            if (result == null)

                return Forbid("You are not allowed to update this mechanic.");

            return Ok(result);

        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)

        {

            var result = await _service.DeleteAsync(id, GetEmail());

            return result == null

                ? Forbid("You are not allowed to delete this mechanic.")

                : Ok(result);

        }



    }
}
