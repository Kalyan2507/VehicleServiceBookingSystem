using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Repositories
{
    public interface ISBookingRepository
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task<bool> UpdateBookingStatusAsync(int bookingId, string status);
    }
}
