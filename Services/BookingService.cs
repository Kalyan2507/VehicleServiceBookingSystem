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
            return bookings.Select(b => new BookingDto
            {
                BookingId = b.Bookingid,
                RegistrationNumber = b.Vehicle.RegistrationNumber,
                VehicleId = b.Vehicle.VehicleId,
                Make = b.Vehicle.Make,
                ServiceTypeDescription = b.ServiceType.Description,
                MechanicName = b.Mechanic?.MechanicName ?? "Not Assigned",
                Date = b.Date,
                TimeSlot = b.TimeSlot,
                ServiceStatus = b.Status,
                ServiceTypeId = b.ServiceTypeId,
                ServiceCenterId = b.ServiceCenterId,
                ServiceCenterName = b.ServiceCenter.ServiceCenterName
            });
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var booking = await _repo.GetDetailedByIdAsync(id);
            if (booking == null || booking.UserId != user.UserId)
                return null;

            return new BookingDto
            {
                BookingId = booking.Bookingid,
                RegistrationNumber = booking.Vehicle?.RegistrationNumber,
                VehicleId = booking.Vehicle?.VehicleId ?? 0,
                Make = booking.Vehicle.Make,
                ServiceTypeDescription = booking.ServiceType?.Description,
                MechanicName = booking.Mechanic?.MechanicName ?? "Not Assigned",
                Date = booking.Date,
                TimeSlot = booking.TimeSlot,
                ServiceStatus = booking.Status,
                ServiceTypeId = booking.ServiceTypeId,
                ServiceCenterId = booking.ServiceCenterId,
                ServiceCenterName = booking.ServiceCenter.ServiceCenterName
            };
        }

        //public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, string email)
        //{
        //    var user = await _userRepo.GetUserByEmailAsync(email);
        //    var mechanic = await _repo.GetAvailableMechanicAsync(dto.ServiceCenterId ?? 0);

        //    var booking = new Booking
        //    {
        //        UserId = user.UserId,
        //        VehicleId = dto.VehicleId,
        //        ServiceCenterId = dto.ServiceCenterId,
        //        ServiceTypeId = (int)dto.ServiceTypeId,
        //        Date = dto.Date,
        //        TimeSlot = dto.TimeSlot,
        //        Status = "Pending",
        //        MechanicId = mechanic?.Mechanicid ?? 0
        //    };

        //    await _repo.AddAsync(booking);
        //    await _repo.SaveChangesAsync();

        //    var detailedBooking = await _repo.GetDetailedByIdAsync(booking.Bookingid);

        //    return _mapper.Map<BookingDto>(detailedBooking);
        //}

        //public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, string email)
        //{
        //    var user = await _userRepo.GetUserByEmailAsync(email);
        //    var mechanic = await _repo.GetAvailableMechanicAsync(dto.ServiceCenterId ?? 0);

        //    var booking = new Booking
        //    {
        //        UserId = user.UserId,
        //        VehicleId = dto.VehicleId,
        //        ServiceCenterId = dto.ServiceCenterId,
        //        ServiceTypeId = (int)dto.ServiceTypeId,
        //        Date = dto.Date,
        //        TimeSlot = dto.TimeSlot,
        //        Status = "Pending",
        //        MechanicId = mechanic?.Mechanicid // null if not assigned
        //    };

        //    await _repo.AddAsync(booking);
        //    await _repo.SaveChangesAsync();

        //    // Fetch with full details
        //    var detailedBooking = await _repo.GetDetailedByIdAsync(booking.Bookingid);

        //    if (detailedBooking == null) return null;

        //    return new BookingDto
        //    {
        //        BookingId = detailedBooking.Bookingid,
        //        RegistrationNumber = detailedBooking.Vehicle?.RegistrationNumber,
        //        VehicleId = detailedBooking.Vehicle?.VehicleId ?? 0,
        //        Make = detailedBooking.Vehicle?.Make,
        //        ServiceTypeDescription = detailedBooking.ServiceType?.Description,
        //        MechanicName = detailedBooking.Mechanic?.MechanicName ?? "Not Assigned",
        //        Date = detailedBooking.Date,
        //        TimeSlot = detailedBooking.TimeSlot,
        //        ServiceStatus = detailedBooking.Status,
        //        ServiceTypeId = detailedBooking.ServiceTypeId,
        //        ServiceCenterId = detailedBooking.ServiceCenterId,
        //        ServiceCenterName = detailedBooking.ServiceCenter?.ServiceCenterName
        //    };
        //}

        public async Task<BookingDto?> CreateBookingAsync(CreateBookingDto dto, string email)

        {

            var user = await _userRepo.GetUserByEmailAsync(email);

            if (user == null)

                throw new Exception("User not found");

            // ✅ Prevent duplicate booking for same vehicle, date, and timeslot

            var allBookings = await _repo.GetAllByUserIdAsync(user.UserId);

            var alreadyBooked = allBookings.Any(b =>

                b.VehicleId == dto.VehicleId &&

                b.Date.Date == dto.Date.Date &&

                //b.TimeSlot == dto.TimeSlot &&

                b.Status != "Cancelled"

            );

            if (alreadyBooked)

                throw new Exception("A booking already exists for this vehicle, date, and time slot.");

            var mechanic = await _repo.GetAvailableMechanicAsync(dto.ServiceCenterId ?? 0);

            var booking = new Booking

            {

                UserId = user.UserId,

                VehicleId = dto.VehicleId,

                ServiceCenterId = dto.ServiceCenterId,

                ServiceTypeId = (int)dto.ServiceTypeId,

                Date = dto.Date,

                TimeSlot = dto.TimeSlot,

                Status = "Pending",

                MechanicId = mechanic?.Mechanicid

            };

            await _repo.AddAsync(booking);

            await _repo.SaveChangesAsync();

            var detailedBooking = await _repo.GetDetailedByIdAsync(booking.Bookingid);

            if (detailedBooking == null) return null;

            return new BookingDto

            {

                BookingId = detailedBooking.Bookingid,

                RegistrationNumber = detailedBooking.Vehicle?.RegistrationNumber,

                VehicleId = detailedBooking.Vehicle?.VehicleId ?? 0,

                Make = detailedBooking.Vehicle?.Make,

                ServiceTypeDescription = detailedBooking.ServiceType?.Description,

                MechanicName = detailedBooking.Mechanic?.MechanicName ?? "Not Assigned",

                Date = detailedBooking.Date,

                TimeSlot = detailedBooking.TimeSlot,

                ServiceStatus = detailedBooking.Status,

                ServiceTypeId = detailedBooking.ServiceTypeId,

                ServiceCenterId = detailedBooking.ServiceCenterId,

                ServiceCenterName = detailedBooking.ServiceCenter?.ServiceCenterName

            };

        }


        public async Task<bool> UpdateBookingAsync(int id, CreateBookingDto dto)
        {
            var booking = await _repo.GetByIdAsync(id);
            if (booking == null) return false;

            booking.VehicleId = dto.VehicleId;
            booking.ServiceCenterId = dto.ServiceCenterId;
            booking.ServiceTypeId = (int)dto.ServiceTypeId;
            booking.Date = dto.Date;
            booking.TimeSlot = dto.TimeSlot;

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