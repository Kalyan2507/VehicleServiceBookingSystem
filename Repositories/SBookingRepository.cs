using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Repositories
{
    public class SBookingRepository : ISBookingRepository
    {
        private readonly VehicleServiceBookContext _context;

        public SBookingRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            return await _context.Bookings
                .Select(b => new BookingDto
                {
                    BookingId = b.Bookingid,
                    Date = (DateTime)b.Date,
                    TimeSlot = b.TimeSlot,
                    Status = b.Status,
                    VehicleId = b.VehicleId ?? 0,
                    ServiceCenterId = b.ServiceCenterId
                })
                .ToListAsync();
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return null;

            return new BookingDto
            {
                BookingId = booking.Bookingid,
                Date = (DateTime)booking.Date,
                TimeSlot = booking.TimeSlot,
                Status = booking.Status,
                VehicleId = booking.VehicleId ?? 0,
                ServiceCenterId = booking.ServiceCenterId
            };
        }

        public async Task<bool> UpdateBookingStatusAsync(int bookingId, string status)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
