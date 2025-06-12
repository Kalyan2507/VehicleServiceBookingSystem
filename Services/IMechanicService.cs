using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Services
{
    public interface IMechanicService
    {
        Task<IEnumerable<MechanicDto>> GetMyMechanicsAsync(string email);
        Task<MechanicDto?> GetByIdAsync(int id, string email);
        Task<MechanicDto?> CreateAsync(CreateMechanicDto dto, string email);
        Task<MechanicDto?> UpdateAsync(int id, CreateMechanicDto dto, string email);
        Task<string?> DeleteAsync(int id, string email);
    }
}
