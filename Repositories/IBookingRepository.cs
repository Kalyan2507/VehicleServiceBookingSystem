using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllByUserIdAsync(int userId);
        Task<Booking?> GetByIdAsync(int id);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();
        
        
      
    }
}
