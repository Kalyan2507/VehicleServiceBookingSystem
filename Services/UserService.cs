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

        // Unified Account Registration for both User and ServiceCenter
        public async Task<bool> RegisterAccountAsync(RegisterAccountDto dto)
        {
            // Check if user already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
                return false;

            // Map to Registration
            var registration = _mapper.Map<Registration>(dto);
            registration.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            registration.Role = dto.Role;

            await _userRepository.AddUserAsync(registration);
            await _userRepository.SaveChangeAsync();

            // If ServiceCenter, add service center details
            if (dto.Role == "ServiceCenter")
            {
                var serviceCenter = new ServiceCenter
                {
                    UserId = registration.UserId,
                    ServiceCenterName = dto.ServiceCenterName,
                    ServiceCenterLocation = dto.ServiceCenterLocation,
                    ServiceCenterContact = dto.ServiceCenterContact
                };
                await _serviceCenterRepository.AddAsync(serviceCenter);
                await _serviceCenterRepository.SaveChangesAsync();
            }

            return true;
        }


        //public async Task<UserDto> RegisterUserAsync(RegisterUserDto dto)
        //{
        //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.PasswordHash);
        //    var registration = new Registration
        //    {
        //        Name = dto.Name,
        //        Email = dto.Email,
        //        Phone = dto.Phone,
        //        Address = dto.Address,
        //        PasswordHash = hashedPassword,
        //        Role = dto.Role
        //    };

        //    await _userRepository.AddUserAsync(registration);
        //    await _userRepository.SaveChangeAsync();

        //    return _mapper.Map<UserDto>(registration);
        //}

        public async Task<object> GetProfileAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return null;

            if (user.Role == "ServiceCenter")
            {
                var serviceCenter = await _serviceCenterRepository.GetByUserIdAsync(user.UserId);
                if (serviceCenter == null)
                    return null;

                return new ProfileDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    ServiceCenterId = serviceCenter.ServiceCenterId,
                    ServiceCenterName = serviceCenter.ServiceCenterName,
                    ServiceCenterLocation = serviceCenter.ServiceCenterLocation,
                    ServiceCenterContact = serviceCenter.ServiceCenterContact
                };
            }
            else
            {
                // Populate only user-related fields for regular users
                return new UserDto
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Address = user.Address
                    // ServiceCenter fields remain null/default
                };
            }
        }

        public async Task<bool> UpdateProfileAsync(string email, UpdateAccountDto dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null)
                return false;

            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.Address = dto.Address;

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
