using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllByUserIdAsync(int userId);
        Task<Booking?> GetByIdAsync(int id);
        Task<Booking?> GetDetailedByIdAsync(int id);
        Task<IEnumerable<Booking>> GetDetailedByUserIdAsync(int userId);
        Task<Mechanic?> GetAvailableMechanicAsync(int serviceCenterId);
        Task<IEnumerable<ServiceCenter>> GetAllServiceCentersAsync();
        Task<IEnumerable<ServiceType>> GetAllServiceTypesAsync();
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<bool> SaveChangesAsync();



    }
}
