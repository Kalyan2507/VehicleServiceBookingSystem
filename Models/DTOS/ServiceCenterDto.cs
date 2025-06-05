using VehicleServiceBook.Models.DTOS;

namespace VehicleServiceBook.Models.DTOS
{
    public class ServiceCenterDto
    {
        public int ServiceCenterId { get; set; }

        public int? UserId { get; set; }

        public string? ServiceCenterName { get; set; }

        public string? ServiceCenterLocation { get; set; }

        public string? ServiceCenterContact { get; set; }
    }
}
