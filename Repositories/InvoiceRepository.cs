using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public class InvoiceRepository:IInvoiceRepository
    {
        private readonly VehicleServiceBookContext _context;

        public InvoiceRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByUserIdAsync(int userId)
        {
            return await _context.Invoices
                .Include(i => i.Booking)
                .ThenInclude(b => b.ServiceType)
                .Where(i => i.Booking.UserId == userId)
                .ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdForUserAsync(int id, int userId)
        {
            return await _context.Invoices
                .Include(i => i.Booking)
                .ThenInclude(b => b.ServiceType)
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.Booking.UserId == userId);
        }

        public async Task<Invoice?> GetExistingInvoiceByBookingIdAsync(int bookingId)
        {
            return await _context.Invoices
                .FirstOrDefaultAsync(i => i.BookingId == bookingId);
        }

        public async Task<ServiceType?> GetServiceTypeByBookingIdAsync(int serviceTypeId)
        {
            return await _context.ServiceTypes
                .FirstOrDefaultAsync(s => s.ServiceTypeId == serviceTypeId);
        }

        public async Task<Booking?> GetBookingByIdForUserAsync(int bookingId, int userId)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.Bookingid == bookingId && b.UserId == userId);
        }

        public async Task AddAsync(Invoice invoice)
        {
            await _context.Invoices.AddAsync(invoice);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
