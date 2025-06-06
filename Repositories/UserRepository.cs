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
            return await _context.Registrations.FirstOrDefaultAsync( u=> u.Email==email && u.PasswordHash==password);
        }

        public async Task<Registration> GetUserByIdAsync(int id)
        {
            return await _context.Registrations.FindAsync(id);
        }

        public async Task<bool> SaveChangeAsync()
        {
            try
            {
                return (await _context.SaveChangesAsync()) > 0;
            }
            catch (DbUpdateException ex) // Catch the EF Core update exception
            {
                // Check if the inner exception is a SQL Server exception
                if (ex.InnerException is SqlException sqlException)
                {
                    // Error Number 2627 indicates a UNIQUE KEY constraint violation
                    if (sqlException.Number == 2627)
                    {
                        // Throw your custom exception, which your middleware will then catch
                        throw new CustomValidationException(
                            "A user with this email already exists. Please use a different email.",
                            ex, // Pass original exception as inner exception for full details in dev logs
                            statusCode: (int)System.Net.HttpStatusCode.Conflict // HTTP 409 Conflict
                        );
                    }
                    // If it's another type of SqlException, just re-throw it as a general app error
                    // This will then be caught by your global ExceptionMiddleware as a 500 error.
                    else
                    {
                        throw new ApplicationException("An unexpected database error occurred.", ex);
                    }
                }
                // If it's a DbUpdateException but not a SqlException, re-throw as general app error
                else
                {
                    throw new ApplicationException("An error occurred while saving changes.", ex);
                }
            }
            catch (Exception ex) // Catch any other truly unexpected errors during save
            {
                // Re-throw as a general application error, which your global middleware will handle as a 500.
                throw new ApplicationException("An unexpected error occurred during the save operation.", ex);
            }
        }
    }
}
