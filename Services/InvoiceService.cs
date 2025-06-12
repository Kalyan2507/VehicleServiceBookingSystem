using AutoMapper;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public InvoiceService(IInvoiceRepository repo, IUserRepository userRepo, IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InvoiceDto>> GetAllAsync(string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var invoices = await _repo.GetInvoicesByUserIdAsync(user.UserId);

            return invoices.Select(invoice => new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                BookingId = (int)invoice.BookingId,
                ServiceTypeId = invoice.ServiceTypeId,
                TotalAmount = (decimal)invoice.TotalAmount,
                PaymentStatus = invoice.PaymentStatus,
                PaymentDate = invoice.Date,
                ServiceTypeDescription = invoice.ServiceType.Description,
                BookingDate = invoice.Booking.Date,
                BookingStatus = invoice.Booking.Status
            }).ToList();
        }

        public async Task<InvoiceDto?> GetByIdAsync(int id, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var invoice = await _repo.GetInvoiceByIdForUserAsync(id, user.UserId);

            if (invoice == null) return null;

            return new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                BookingId = (int)invoice.BookingId,
                ServiceTypeId = invoice.ServiceTypeId,
                TotalAmount = (decimal)invoice.TotalAmount,
                PaymentStatus = invoice.PaymentStatus,
                PaymentDate = invoice.Date,
                ServiceTypeDescription = invoice.ServiceType.Description,
                BookingDate = invoice.Booking.Date,
                BookingStatus = invoice.Booking.Status
            };
        }

        public async Task<object?> CreateAsync(CreateInvoiceDto dto, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);

            var existing = await _repo.GetExistingInvoiceByBookingIdAsync(dto.BookingId);
            if (existing != null)
                return "Already paid";

            var booking = await _repo.GetBookingByIdForUserAsync(dto.BookingId, user.UserId);
            if (booking == null)
                return null;

            var serviceType = await _repo.GetServiceTypeByBookingIdAsync(booking.ServiceTypeId);
            if (serviceType == null)
                return "Service type not found";

            var invoice = new Invoice
            {
                BookingId = booking.Bookingid,
                ServiceTypeId = booking.ServiceTypeId,
                TotalAmount = (double?)serviceType.Price,
                PaymentStatus = "Paid",
                Date = DateTime.Now
            };

            await _repo.AddAsync(invoice);
            await _repo.SaveChangesAsync();

            return new
            {
                invoice.InvoiceId,
                invoice.BookingId,
                invoice.TotalAmount,
                invoice.PaymentStatus,
                invoice.Date
            };
        }

    }
}
