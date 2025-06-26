using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public interface IServiceCenterRepository
    {
        Task<IEnumerable<ServiceCenter>> GetAllAsync();
        Task<ServiceCenter> GetByIdAsync(int id);
        Task<ServiceCenter> GetByUserIdAsync(int userId);
        Task AddAsync(ServiceCenter serviceCenter);
        Task<IEnumerable<Booking>> GetBookingsByServiceCenterIdAsync(int serviceCenterId);
        Task<Booking> GetBookingByIdAsync(int bookingId);
        Task<Mechanic> GetMechanicByIdAsync(int mechanicId);
        Task<bool> SaveChangesAsync();
    }
}
