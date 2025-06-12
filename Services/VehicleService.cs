using AutoMapper;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository vehicleRepo, IUserRepository userRepo, IMapper mapper)
        {
            _vehicleRepo = vehicleRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VehicleDto>> GetAllAsync(string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var vehicles = await _vehicleRepo.GetAllByUserIdAsync(user.UserId);
            return _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDto?> GetByIdAsync(int id, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (vehicle == null || vehicle.UserId != user.UserId)
                return null;
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<VehicleDto> CreateAsync(CreateVehicleDto dto, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var vehicle = _mapper.Map<Vehicle>(dto);
            vehicle.UserId = user.UserId;

            await _vehicleRepo.AddAsync(vehicle);
            await _vehicleRepo.SaveChangesAsync();
            return _mapper.Map<VehicleDto>(vehicle);
        }

        public async Task<string?> UpdateAsync(int id, CreateVehicleDto dto, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (vehicle == null || vehicle.UserId != user.UserId)
                return null;

            vehicle.Make = dto.Make;
            vehicle.Model = dto.Model;
            vehicle.Year = dto.Year;
            vehicle.RegistrationNumber = dto.RegistrationNumber;

            await _vehicleRepo.UpdateAsync(vehicle);
            await _vehicleRepo.SaveChangesAsync();
            return "Vehicle Updated Successfully";
        }

        public async Task<string?> DeleteAsync(int id, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var vehicle = await _vehicleRepo.GetByIdAsync(id);
            if (vehicle == null || vehicle.UserId != user.UserId)
                return null;

            await _vehicleRepo.DeleteAsync(id);
            await _vehicleRepo.SaveChangesAsync();
            return "Vehicle Deleted Successfully";
        }
    }
}
