using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VehicleServiceBookContext _context;

        public VehicleRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
        }

        public async Task DeleteAsync(int id)
        {
            var v = await GetByIdAsync(id);
            if (v != null)
                _context.Vehicles.Remove(v);
        }

        public async Task<IEnumerable<Vehicle>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Vehicles.Where(v => v.UserId == userId).ToListAsync();
        }

        public async Task<Vehicle> GetByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
        }
    }
}
