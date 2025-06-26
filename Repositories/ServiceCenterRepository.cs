using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public class ServiceCenterRepository : IServiceCenterRepository
    {
        private readonly VehicleServiceBookContext _context;

        public ServiceCenterRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ServiceCenter serviceCenter)
        {
            await _context.ServiceCenters.AddAsync(serviceCenter);
        }

        public async Task<IEnumerable<ServiceCenter>> GetAllAsync()
        {
            return await _context.ServiceCenters.ToListAsync();
        }

        public async Task<ServiceCenter> GetByIdAsync(int id)
        {
            return await _context.ServiceCenters.FindAsync(id);
        }

        public async Task<ServiceCenter> GetByUserIdAsync(int userId)
        {
            return await _context.ServiceCenters.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByServiceCenterIdAsync(int serviceCenterId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Registration)
                .Include(b => b.ServiceType)
                .Include(b => b.Invoice)
                .Include(b => b.Mechanic)
                .Where(b => b.ServiceCenterId == serviceCenterId)
                .ToListAsync();
        }

        public async Task<Booking> GetBookingByIdAsync(int bookingId)
        {
            return await _context.Bookings
                .Include(b => b.Mechanic)
                .FirstOrDefaultAsync(b => b.Bookingid == bookingId);
        }

        public async Task<Mechanic> GetMechanicByIdAsync(int mechanicId)
        {
            return await _context.Mechanics.FirstOrDefaultAsync(m => m.Mechanicid == mechanicId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}