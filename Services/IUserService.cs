using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Services
{
    public interface IUserService
    {
        Task<UserDto>RegisterUserAsync(RegisterUserDto dto);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<Registration>AuthenticateAsync(string email,string password);
    }
}
