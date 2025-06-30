using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Services
{
    public interface IInvoiceService
    {
        Task<IEnumerable<InvoiceDto>> GetAllAsync(string email);
        Task<InvoiceDto?> GetByIdAsync(int id, string email);
        Task<object?> CreateAsync(CreateInvoiceDto dto, string email);

        Task<IEnumerable<InvoiceDto>> GetInvoicesForServiceCenterAsync(string email);
        Task<bool> UpdatePaymentStatusAsync(int invoiceId, string status, string email);

    }
}
