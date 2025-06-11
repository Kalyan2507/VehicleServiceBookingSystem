using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Controllers
{
    [Authorize(Roles = "ServiceCenter")]
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IServiceTypeRepository _repository;
        private readonly IMapper _mapper;

        public ServiceTypeController(IServiceTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var types = await _repository.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ServiceTypeDto>>(types));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var type = await _repository.GetByIdAsync(id);
            if (type == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ServiceTypeDto>(type));
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateServiceTypeDto dto)
        {
            var type = _mapper.Map<ServiceType>(dto);
            await _repository.AddAsync(type);
            await _repository.SaveChangesAsync();
            return Ok(_mapper.Map<ServiceTypeDto>(type));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateServiceTypeDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return NotFound();
            existing.Description = dto.Description;
            existing.Price = dto.Price;
            await _repository.UpdateAsync(existing);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var serviceType = await _repository.GetByIdAsync(id);
            if (serviceType == null)
            {
                return NotFound($"No service type found with Id {id}");
            }
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
            return NoContent();
        }
    }
}
