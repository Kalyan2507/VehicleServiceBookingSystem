using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Services
{
    public interface IServiceTypeService
    {
        Task<IEnumerable<ServiceTypeDto>> GetAllAsync();
        Task<ServiceTypeDto?> GetByIdAsync(int id);
        Task<ServiceTypeDto> CreateAsync(CreateServiceTypeDto dto);
        Task<string?> UpdateAsync(int id, CreateServiceTypeDto dto);
        Task<string?> DeleteAsync(int id);
    }
}
