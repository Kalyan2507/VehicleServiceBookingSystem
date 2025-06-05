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
            CreateMap<Registration,UserDto>();
            CreateMap<ServiceCenter,ServiceCenterDto>();
            CreateMap<Mechanic,MechanicDto>();
            CreateMap<CreateMechanicDto,Mechanic>();
            CreateMap<ServiceType,ServiceTypeDto>();
            CreateMap<CreateServiceTypeDto, ServiceType>();
            CreateMap<CreateVehicleDto, Vehicle>();
            CreateMap<Vehicle, VehicleDto>();
            CreateMap<CreateBookingDto, Booking>();
            CreateMap<Booking, BookingDto>();
        }
    }
}
