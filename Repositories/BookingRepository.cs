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
        public async Task AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
        }

        public async Task DeleteAsync(int id)
        {
            var b = await GetByIdAsync(id);
            if (b != null)
            {
                _context.Bookings.Remove(b);
            }
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task<IEnumerable<Booking>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Bookings.Where(b=>b.UserId == userId).ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync()>0;
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
        }
    }
}
