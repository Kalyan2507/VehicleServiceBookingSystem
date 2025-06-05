using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;

namespace VehicleServiceBook.Repositories
{
    public class InvoiceRepository:IInvoiceRepository
    {
        private readonly VehicleServiceBookContext _context;

        public InvoiceRepository(VehicleServiceBookContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.ToListAsync();
        }

        public async Task<Invoice> GetByIdAsync(int id)
        {
            return await _context.Invoices.FindAsync(id);
        }
    }
}
