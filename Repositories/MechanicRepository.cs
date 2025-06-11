using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Repositories
{
    public class MechanicRepository : IMechanicRepository
    {
        private readonly VehicleServiceBookContext _context;

        public MechanicRepository(VehicleServiceBookContext context)

        {

            _context = context;

        }

        public async Task AddAsync(Mechanic mechanic)

        {

            await _context.Mechanics.AddAsync(mechanic);

        }

        public async Task DeleteAsync(int id)

        {

            var mech = await _context.Mechanics.FindAsync(id);

            if (mech != null)

            {

                _context.Mechanics.Remove(mech);

            }

        }

        public async Task<IEnumerable<Mechanic>> GetAllAsync()

        {

            return await _context.Mechanics.ToListAsync();

        }

        public async Task<Mechanic> GetByIdAsync(int id)

        {

            return await _context.Mechanics.FindAsync(id);

        }

        public async Task<bool> SaveChangesAsync()

        {

            return await _context.SaveChangesAsync() > 0;

        }

        public async Task UpdateAsync(Mechanic mechanic)

        {

            _context.Mechanics.Update(mechanic);

        }

    }
}
