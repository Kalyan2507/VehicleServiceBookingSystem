using AutoMapper;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Services
{
    public class ServiceTypeService : IServiceTypeService
    {
        private readonly IServiceTypeRepository _repo;
        private readonly IMapper _mapper;

        public ServiceTypeService(IServiceTypeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServiceTypeDto>> GetAllAsync()
        {
            var types = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<ServiceTypeDto>>(types);
        }

        public async Task<ServiceTypeDto?> GetByIdAsync(int id)
        {
            var type = await _repo.GetByIdAsync(id);
            return type == null ? null : _mapper.Map<ServiceTypeDto>(type);
        }

        public async Task<ServiceTypeDto> CreateAsync(CreateServiceTypeDto dto)
        {
            var type = _mapper.Map<ServiceType>(dto);
            await _repo.AddAsync(type);
            await _repo.SaveChangesAsync();
            return _mapper.Map<ServiceTypeDto>(type);
        }

        public async Task<string?> UpdateAsync(int id, CreateServiceTypeDto dto)
        {
            var type = await _repo.GetByIdAsync(id);
            if (type == null) return null;

            type.Description = dto.Description;
            type.Price = dto.Price;

            await _repo.UpdateAsync(type);
            await _repo.SaveChangesAsync();
            return "ServiceType Updated Successfully";
        }

        public async Task<string?> DeleteAsync(int id)
        {
            var type = await _repo.GetByIdAsync(id);
            if (type == null) return null;

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return "ServiceType Deleted Successfully";
        }
    }
}
