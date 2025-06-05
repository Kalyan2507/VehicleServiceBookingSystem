using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Services
{
    public interface IAuthService
    {
        Task<string>AuthenticateAsync(string email, string password);
    }
}
