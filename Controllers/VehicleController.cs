using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicleRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public VehicleController(IVehicleRepository repo, IUserRepository userRepo, IMapper mapper)
        {
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
            var vehicles = await _repo.GetAllByUserIdAsync(userId.Value);
            return Ok(_mapper.Map<IEnumerable<VehicleDto>>(vehicles));
        }

        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
                return Unauthorized("data not there");
            var user = await _userRepo.GetUserByEmailAsync(email);
            if (user == null || user.Role != "User")
                return Unauthorized();
            var vehicle = await _repo.GetByIdAsync(id);
            return Ok(_mapper.Map<VehicleDto>(vehicle));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateVehicleDto dto)
        {
            var userId = await GetUserIdFromToken();
            var vehicle = _mapper.Map<Vehicle>(dto);
            vehicle.UserId = userId.Value;

            await _repo.AddAsync(vehicle);
            await _repo.SaveChangesAsync();
            return Ok(_mapper.Map<VehicleDto>(vehicle));
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateVehicleDto dto)
        {
            var vehicle = await _repo.GetByIdAsync(id);
            if (vehicle == null) return NotFound();

            vehicle.Make = dto.Make;
            vehicle.Model = dto.Model;
            vehicle.Year = dto.Year;
            vehicle.RegistrationNumber = dto.RegistrationNumber;

            await _repo.UpdateAsync(vehicle);
            await _repo.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle=await _repo.GetByIdAsync(id);
            if (vehicle == null) 
                return NotFound($"No Vehicle found with Id {id}");
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return NoContent();
        }
    }
}
