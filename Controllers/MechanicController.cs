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
    [Authorize(Roles ="ServiceCenter")]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class MechanicController : ControllerBase
    {
        private readonly VehicleServiceBookContext _vehicleServiceBookContext;
        private readonly IMechanicRepository _repo;
        private readonly IUserRepository _userRepository;
        private readonly IServiceCenterRepository _serviceCenterRepository;
        private readonly IMapper _mapper;

        public MechanicController(VehicleServiceBookContext vehicleServiceBookContext, IMechanicRepository repo,IUserRepository userRepository,IServiceCenterRepository serviceCenterRepository,IMapper mapper)
        {
            _vehicleServiceBookContext = vehicleServiceBookContext;
            _repo = repo;
            _userRepository = userRepository;
            _serviceCenterRepository = serviceCenterRepository;
            _mapper = mapper;
        }
        //[HttpGet]
        //public async Task<IActionResult>GetAll()
        //{
        //    var mechanics=await _repo.GetAllAsync();
        //    return Ok(_mapper.Map<IEnumerable<MechanicDto>>(mechanics));
        //}

        [HttpGet("my-mechanics")]
        public async Task<IActionResult> GetByLoggedInServiceCenter()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.Role != "ServiceCenter")
                return Unauthorized("You are not authorized.");

            var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(user.UserId);
            if (serviceCenter == null)
                return NotFound("Service Center not found.");

            var mechanics = await _repo.GetAllAsync();
            var assigned = mechanics.Where(m => m.ServiceCenterId == serviceCenter.ServiceCenterId);

            return Ok(_mapper.Map<IEnumerable<MechanicDto>>(assigned));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null || user.Role != "ServiceCenter")
                return Unauthorized();
            var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(user.UserId);
            if (serviceCenter == null)
                return Forbid("Service Center not found.");

            var mechanic = await _repo.GetByIdAsync(id);
            if (mechanic == null)
                return NotFound("Mechanic not found");
            if (mechanic.ServiceCenterId != serviceCenter.ServiceCenterId)
                return Forbid("You are not authorized to access this mechanic.");

            var dto = _mapper.Map<MechanicDto>(mechanic);
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult>Create(CreateMechanicDto dto)
        {
            var mech=_mapper.Map<Mechanic>(dto);
            await _repo.AddAsync(mech);
            await _repo.SaveChangesAsync();
            return Ok(_mapper.Map<MechanicDto>(mech));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>Update(int id,CreateMechanicDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if(existing == null)
                return NotFound();
            existing.MechanicName=dto.MechanicName;
            existing.Expertise=dto.Expertise;
            existing.ServiceCenterId=dto.ServiceCenterId;
            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();
            return Ok("Mechanic Updated Successfully");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult>Delete(int id)
        {
            var mechanic=await _repo.GetByIdAsync(id);
            if(mechanic == null)
                return NotFound($"No Mechanic found with Id {id}");
            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return Ok("Deleted Successfully");
        }
        
    }
}
