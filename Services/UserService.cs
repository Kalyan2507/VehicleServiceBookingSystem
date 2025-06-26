using System.Numerics;
using AutoMapper;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;
using VehicleServiceBook.Services;

namespace VehicleServiceBook.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IServiceCenterRepository _serviceCenterRepository;

        public UserService(IUserRepository userRepository,IMapper mapper, IServiceCenterRepository serviceCenterRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _serviceCenterRepository = serviceCenterRepository;
        }
        public async Task<Registration> AuthenticateAsync(string email, string password)
        {
            return await _userRepository.GetUserByEmailPasswordAsync(email, password);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> RegisterUserAsync(RegisterUserDto dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);
            var registration = new Registration
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                PasswordHash = hashedPassword,
                Role = dto.Role
            };

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangeAsync();

            if (user.Role == "ServiceCenter")
            {
                var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(user.UserId);
                if (serviceCenter == null)
                    return false;

                serviceCenter.ServiceCenterName = dto.ServiceCenterName;
                serviceCenter.ServiceCenterLocation = dto.ServiceCenterLocation;
                serviceCenter.ServiceCenterContact = dto.ServiceCenterContact;

                await _serviceCenterRepository.AddAsync(serviceCenter); // If you have UpdateAsync, use that instead!
                await _serviceCenterRepository.SaveChangesAsync();
            }

            return true;
        }
    }
}
