using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync(string userEmail);
        Task<BookingDto?> GetBookingByIdAsync(int id, string userEmail);
        Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, string userEmail);
        Task<bool> UpdateBookingAsync(int id, CreateBookingDto dto);
        Task<bool> UpdateStatusAsync(int id, string status, string userEmail);
        Task<IEnumerable<object>> GetServiceCentersAsync();
        Task<IEnumerable<object>> GetServiceTypesAsync();
    }
}
