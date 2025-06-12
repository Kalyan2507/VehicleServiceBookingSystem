using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Services
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleDto>> GetAllAsync(string email);
        Task<VehicleDto?> GetByIdAsync(int id, string email);
        Task<VehicleDto> CreateAsync(CreateVehicleDto dto, string email);
        Task<string?> UpdateAsync(int id, CreateVehicleDto dto, string email);
        Task<string?> DeleteAsync(int id, string email);
    }
}
