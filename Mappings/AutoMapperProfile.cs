using AutoMapper;
using VehicleServiceBook.Models.Domains;
using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Mappings
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterUserDto, Registration>();
            CreateMap<RegisterServiceCenterDto, ServiceCenter>();
            CreateMap<Registration,UserDto>().ReverseMap();
            CreateMap<RegisterAccountDto, RegisterUserDto>()
            .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
            CreateMap<RegisterAccountDto, Registration>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.Role, opt => opt.Ignore());
            CreateMap<ServiceCenter,ServiceCenterDto>();
            CreateMap<Mechanic,MechanicDto>();
            CreateMap<CreateMechanicDto,Mechanic>();
            CreateMap<ServiceType,ServiceTypeDto>();
            CreateMap<CreateServiceTypeDto, ServiceType>();
            CreateMap<CreateVehicleDto, Vehicle>();
            CreateMap<Vehicle, VehicleDto>();
            CreateMap<CreateBookingDto, Booking>();
            CreateMap<Booking, BookingDto>();
            CreateMap<Invoice, InvoiceDto>();
        }
    }
}
