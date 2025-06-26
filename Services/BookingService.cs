using AutoMapper;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository repo, IUserRepository userRepo, IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync(string userEmail)
        {
            var user = await _userRepo.GetUserByEmailAsync(userEmail);
            var bookings = await _repo.GetDetailedByUserIdAsync(user.UserId);
            return bookings.Select(b => _mapper.Map<BookingDto>(b));
            //new BookingDto
            //{
            //    BookingId = b.Bookingid,
            //    RegistrationNumber = b.Vehicle.RegistrationNumber,
            //    VehicleId = b.Vehicle.VehicleId,
            //    Make = b.Vehicle.Make,
            //    ServiceTypeDescription = b.ServiceType.Description,
            //    MechanicName = b.Mechanic?.MechanicName ?? "Not Assigned",
            //    Date = b.Date,
            //    TimeSlot = b.TimeSlot,
            //    ServiceStatus = b.Status,
            //    ServiceTypeId = b.ServiceTypeId,
            //    ServiceCenterId = b.ServiceCenterId,
            //    ServiceCenterName = b.ServiceCenter.ServiceCenterName
            //}
            
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var booking = await _repo.GetDetailedByIdAsync(id);
            if (booking == null || booking.UserId != user.UserId)
                return null;

            return _mapper.Map<BookingDto>(booking);
            //new BookingDto
            //{
            //    BookingId = booking.Bookingid,
            //    RegistrationNumber = booking.Vehicle?.RegistrationNumber,
            //    VehicleId = booking.Vehicle?.VehicleId ?? 0,
            //    ServiceTypeDescription = booking.ServiceType?.Description,
            //    MechanicName = booking.Mechanic?.MechanicName ?? "Not Assigned",
            //    Date = booking.Date,
            //    TimeSlot = booking.TimeSlot,
            //    ServiceStatus = booking.Status,
            //    ServiceTypeId = booking.ServiceTypeId,
            //    ServiceCenterId = booking.ServiceCenterId
            //};
        }

        public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var mechanic = await _repo.GetAvailableMechanicAsync(dto.ServiceCenterId ?? 0);


            var booking = _mapper.Map<Booking>(dto);
            booking.UserId = user.UserId;
            booking.MechanicId = mechanic?.Mechanicid ?? 0;


            await _repo.AddAsync(booking);
            await _repo.SaveChangesAsync();

            return _mapper.Map<BookingDto>(booking);
        }

        public async Task<bool> UpdateBookingAsync(int id, CreateBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id);
            if (booking == null) return false;

            //booking.VehicleId = dto.VehicleId;
            //booking.ServiceCenterId = dto.ServiceCenterId;
            //booking.ServiceTypeId = (int)dto.ServiceTypeId;
            //booking.Date = dto.Date;
            //booking.TimeSlot = dto.TimeSlot;

            _mapper.Map(dto, booking);

            await _repo.UpdateAsync(booking);
            return await _repo.SaveChangesAsync();
        }

        public async Task<bool> UpdateStatusAsync(int id, string status, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var booking = await _repo.GetByIdAsync(id);
            if (booking == null || booking.UserId != user.UserId)
                return false;

            booking.Status = status;
            await _repo.UpdateAsync(booking);
            return await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<object>> GetServiceCentersAsync()
        {
            var result = await _repo.GetAllServiceCentersAsync();
            return result.Select(sc => new
            {
                sc.ServiceCenterId,
                sc.ServiceCenterName,
                sc.ServiceCenterLocation,
                sc.ServiceCenterContact
            });
        }

        public async Task<IEnumerable<object>> GetServiceTypesAsync()
        {
            var result = await _repo.GetAllServiceTypesAsync();
            return result.Select(st => new
            {
                st.ServiceTypeId,
                st.Description,
                st.Price
            });
        }
    }
}
