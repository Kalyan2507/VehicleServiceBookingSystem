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

        public UserService(IUserRepository userRepository,IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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

            await _userRepository.AddUserAsync(registration);
            await _userRepository.SaveChangeAsync();

            return _mapper.Map<UserDto>(registration);
        }
    }
}
