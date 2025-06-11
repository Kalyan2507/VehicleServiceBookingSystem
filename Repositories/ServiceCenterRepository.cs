using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Repositories
{
    public class ServiceCenterRepository : IServiceCenterRepository
    {
        private readonly VehicleServiceBookContext _context;

        public ServiceCenterRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ServiceCenter serviceCenter)
        {
            await _context.ServiceCenters.AddAsync(serviceCenter);
        }

        public async Task<IEnumerable<ServiceCenter>> GetAllAsync()
        {
            return await _context.ServiceCenters.ToListAsync();
        }

        public async Task<ServiceCenter> GetByIdAsync(int id)
        {
            return await _context.ServiceCenters.FindAsync(id);
        }

        public async Task<ServiceCenter> GetByUserIdAsync(int userId)
        {
            return await _context.ServiceCenters.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
