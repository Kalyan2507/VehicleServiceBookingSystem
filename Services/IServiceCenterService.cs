using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Services
{
    public interface IServiceCenterService
    {
        Task<ServiceCenterDto> RegisterServiceCenterAsync(RegisterServiceCenterDto dto);
        Task<ServiceCenterDto> GetByUserIdAsync(int userId);
        Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(int userId);
        Task<bool> UpdateBookingStatusAsync(int userId, int bookingId, string status);
        Task<bool> AssignMechanicAsync(int userId, int bookingId, int mechanicId);
    }
}
