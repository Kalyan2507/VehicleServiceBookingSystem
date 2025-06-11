using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public class ServiceTypeRepository : IServiceTypeRepository
    {
        private readonly VehicleServiceBookContext _context;

        public ServiceTypeRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }
        public async Task AddAsync(ServiceType serviceType)
        {
            await _context.ServiceTypes.AddAsync(serviceType);
        }

        public async Task DeleteAsync(int id)
        {
            var serviceType = await GetByIdAsync(id);
            if (serviceType != null)
            {
                _context.ServiceTypes.Remove(serviceType);
            }
        }

        public async Task<IEnumerable<ServiceType>> GetAllAsync()
        {
            return await _context.ServiceTypes.ToListAsync();
        }

        public async Task<ServiceType> GetByIdAsync(int id)
        {
            return await _context.ServiceTypes.FindAsync(id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateAsync(ServiceType serviceType)
        {
            _context.ServiceTypes.Update(serviceType);
        }
    }
}
