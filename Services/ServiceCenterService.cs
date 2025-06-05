using AutoMapper;
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
        private readonly IMapper _mapper;

        public ServiceCenterService(IServiceCenterRepository serviceCenterRepository,IUserRepository userRepository,IMapper mapper)
        {
            _serviceCenterRepository = serviceCenterRepository;
            _userRepository = userRepository;
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

            // Register the service center
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
    }
}
