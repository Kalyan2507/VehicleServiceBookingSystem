using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Services
{
    public interface IServiceCenterService
    {
        Task<ServiceCenterDto> RegisterServiceCenterAsync(RegisterServiceCenterDto dto);
        Task<ServiceCenterDto>GetByUserIdAsync(int userId);
    }
}
