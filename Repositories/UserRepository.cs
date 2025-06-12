using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Middleware;
using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly VehicleServiceBookContext _context;

        public UserRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }
        public async Task AddUserAsync(Registration user)
        {
            await _context.Registrations.AddAsync(user);
        }

        public async Task<IEnumerable<Registration>> GetAllUserAsync()
        {
            return await _context.Registrations.ToListAsync();
        }

        public async Task<Registration> GetUserByEmailAsync(string email)
        {
            return await _context.Registrations.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Registration> GetUserByEmailPasswordAsync(string email, string password)
        {
            return await _context.Registrations.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == password);
        }

        public async Task<Registration> GetUserByIdAsync(int id)
        {
            return await _context.Registrations.FindAsync(id);
        }

        public async Task<bool> SaveChangeAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task UpdateAsync(Registration user)
        {
            _context.Registrations.Update(user);
        }
    }
}