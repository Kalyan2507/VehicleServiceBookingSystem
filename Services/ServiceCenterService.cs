using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Services;

namespace VehicleServiceBook.Services
{
    public class ServiceCenterService : IServiceCenterService
    {
        private readonly IServiceCenterRepository _serviceCenterRepository;
        private readonly IUserRepository _userRepository;
        private readonly VehicleServiceBookContext _context;
        private readonly IMapper _mapper;

        public ServiceCenterService(IServiceCenterRepository serviceCenterRepository,
                                     IUserRepository userRepository,
                                     VehicleServiceBookContext context,
                                     IMapper mapper)
        {
            _serviceCenterRepository = serviceCenterRepository;
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceCenterDto> GetByUserIdAsync(int userId)
        {
            var result = await _serviceCenterRepository.GetByUserIdAsync(userId);
            return _mapper.Map<ServiceCenterDto>(result);
        }

        public async Task<ServiceCenterDto> RegisterServiceCenterAsync(RegisterServiceCenterDto dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.User.PasswordHash);
            var user = new Registration
            {
                Name = dto.User.Name,
                Email = dto.User.Email,
                Phone = dto.User.Phone,
                Address = dto.User.Address,
                PasswordHash = hashedPassword,
                Role = "ServiceCenter"
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangeAsync();

            var serviceCenter = new ServiceCenter
            {
                UserId = user.UserId,
                ServiceCenterName = dto.ServiceCenterName,
                ServiceCenterLocation = dto.ServiceCenterLocation,
                ServiceCenterContact = dto.ServiceCenterContact
            };

            await _serviceCenterRepository.AddAsync(serviceCenter);
            await _serviceCenterRepository.SaveChangesAsync();

            return _mapper.Map<ServiceCenterDto>(serviceCenter);
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsAsync(int userId)
        {
            var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(userId);
            if (serviceCenter == null) return null;

            var appointments = await _context.Bookings
                .Include(b => b.Vehicle)
                .Include(b => b.Registration)
                .Include(b => b.ServiceType)
                .Include(b => b.Invoice)
                .Include(b => b.Mechanic)
                .Where(b => b.ServiceCenterId == serviceCenter.ServiceCenterId)
                .ToListAsync();

            return appointments.Select(b => new AppointmentDto
            {
                RegistrationNumber = b.Vehicle.RegistrationNumber,
                CustomerName = b.Registration.Name,
                MechanicName = b.Mechanic?.MechanicName ?? "Not Assigned",
                ServiceTypeDescription = b.ServiceType.Description,
                Date = b.Date,
                TimeSlot = b.TimeSlot,
                Price = (decimal)b.ServiceType.Price,
                ServiceStatus = b.Status ?? "Pending",
                PaymentStatus = b.Invoice?.PaymentStatus ?? "Pending"
            }).ToList();
        }

        public async Task<bool> UpdateBookingStatusAsync(int userId, int bookingId, string status)
        {
            var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(userId);
            if (serviceCenter == null) return false;

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Bookingid == bookingId);
            if (booking == null || booking.ServiceCenterId != serviceCenter.ServiceCenterId) return false;

            booking.Status = status;
            _context.Bookings.Update(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AssignMechanicAsync(int userId, int bookingId, int mechanicId)
        {
            var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(userId);
            if (serviceCenter == null) return false;

            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Bookingid == bookingId);
            var mechanic = await _context.Mechanics.FirstOrDefaultAsync(m => m.Mechanicid == mechanicId);

            if (booking == null || mechanic == null || mechanic.ServiceCenterId != serviceCenter.ServiceCenterId ||
                booking.ServiceCenterId != serviceCenter.ServiceCenterId)
                return false;

            booking.MechanicId = mechanicId;
            _context.Bookings.Update(booking);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
