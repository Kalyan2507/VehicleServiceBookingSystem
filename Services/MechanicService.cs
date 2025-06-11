using AutoMapper;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Repositories;

namespace VehicleServiceBook.Services
{
    public class MechanicService : IMechanicService
    {
        private readonly IMechanicRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IServiceCenterRepository _serviceCenterRepo;
        private readonly IMapper _mapper;

        public MechanicService(
            IMechanicRepository repo,
            IUserRepository userRepo,
            IServiceCenterRepository serviceCenterRepo,
            IMapper mapper)
        {
            _repo = repo;
            _userRepo = userRepo;
            _serviceCenterRepo = serviceCenterRepo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MechanicDto>> GetMyMechanicsAsync(string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var serviceCenter = await _serviceCenterRepo.GetByUserIdAsync(user.UserId);

            var mechanics = await _repo.GetAllAsync();
            var filtered = mechanics.Where(m => m.ServiceCenterId == serviceCenter.ServiceCenterId);

            return _mapper.Map<IEnumerable<MechanicDto>>(filtered);
        }

        public async Task<MechanicDto?> GetByIdAsync(int id, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var serviceCenter = await _serviceCenterRepo.GetByUserIdAsync(user.UserId);

            var mechanic = await _repo.GetByIdAsync(id);
            if (mechanic == null || mechanic.ServiceCenterId != serviceCenter.ServiceCenterId)
                return null;

            return _mapper.Map<MechanicDto>(mechanic);
        }

        public async Task<MechanicDto?> CreateAsync(CreateMechanicDto dto, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var serviceCenter = await _serviceCenterRepo.GetByUserIdAsync(user.UserId);

            var mechanic = _mapper.Map<Mechanic>(dto);
            mechanic.ServiceCenterId = serviceCenter.ServiceCenterId;

            await _repo.AddAsync(mechanic);
            await _repo.SaveChangesAsync();

            return _mapper.Map<MechanicDto>(mechanic);
        }

        public async Task<MechanicDto?> UpdateAsync(int id, CreateMechanicDto dto, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var serviceCenter = await _serviceCenterRepo.GetByUserIdAsync(user.UserId);

            var existing = await _repo.GetByIdAsync(id);
            if (existing == null || existing.ServiceCenterId != serviceCenter.ServiceCenterId)
                return null;

            existing.MechanicName = dto.MechanicName;
            existing.Expertise = dto.Expertise;

            await _repo.UpdateAsync(existing);
            await _repo.SaveChangesAsync();

            return _mapper.Map<MechanicDto>(existing);
        }

        public async Task<string?> DeleteAsync(int id, string email)
        {
            var user = await _userRepo.GetUserByEmailAsync(email);
            var serviceCenter = await _serviceCenterRepo.GetByUserIdAsync(user.UserId);

            var mech = await _repo.GetByIdAsync(id);
            if (mech == null || mech.ServiceCenterId != serviceCenter.ServiceCenterId)
                return null;

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
            return "Deleted Successfully";
        }
    }
}
