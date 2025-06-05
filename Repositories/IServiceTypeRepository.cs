using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Repositories
{
    public interface IServiceTypeRepository
    {
        Task<IEnumerable<ServiceType>> GetAllAsync();
        Task<ServiceType> GetByIdAsync(int id);
        Task AddAsync(ServiceType serviceType);
        Task UpdateAsync(ServiceType serviceType);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
