using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<Registration>> GetAllUserAsync();
        Task<Registration> GetUserByIdAsync(int id);
        Task<Registration> GetUserByEmailPasswordAsync(string email, string password);
        Task<Registration> GetUserByEmailAsync(string email);
        Task AddUserAsync(Registration user);
        Task UpdateAsync(Registration user);
        Task<bool> SaveChangeAsync();
    }
}