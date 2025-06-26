using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly VehicleServiceBookContext _context;

        public BookingRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Booking>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.ServiceType)
                .Include(b => b.Mechanic)
                .Include(b => b.ServiceCenter)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetDetailedByUserIdAsync(int userId)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.ServiceType)
                .Include(b => b.Mechanic)
                .Include(b => b.ServiceCenter)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.Bookingid == id);
        }

        public async Task<Booking?> GetDetailedByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.ServiceType)
                .Include(b => b.Mechanic)
                .Include(b => b.ServiceCenter)
                .FirstOrDefaultAsync(b => b.Bookingid == id);
        }

        public async Task<Mechanic?> GetAvailableMechanicAsync(int serviceCenterId)
        {
            return await _context.Mechanics
                .FirstOrDefaultAsync(m => m.ServiceCenterId == serviceCenterId);
        }

        public async Task<IEnumerable<ServiceCenter>> GetAllServiceCentersAsync()
        {
            return await _context.ServiceCenters.ToListAsync();
        }

        public async Task<IEnumerable<ServiceType>> GetAllServiceTypesAsync()
        {
            return await _context.ServiceTypes.ToListAsync();
        }

        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await GetByIdAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
