using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public interface IMechanicRepository
    {
        Task<IEnumerable<Mechanic>> GetAllAsync();
        Task<Mechanic> GetByIdAsync(int id);
        Task AddAsync(Mechanic mechanic);
        Task UpdateAsync(Mechanic mechanic);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
