using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetInvoicesByUserIdAsync(int userId);
        Task<Invoice?> GetInvoiceByIdForUserAsync(int id, int userId);
        Task<Invoice?> GetExistingInvoiceByBookingIdAsync(int bookingId);
        Task<ServiceType?> GetServiceTypeByBookingIdAsync(int serviceTypeId);
        Task<Booking?> GetBookingByIdForUserAsync(int bookingId, int userId);
        Task AddAsync(Invoice invoice);
        Task<bool> SaveChangesAsync();
    }
}
