namespace VehicleServiceBook.Models.DTOS
{
    public class UpdateAccountDto
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string? ServiceCenterName { get; set; }
        public string? ServiceCenterLocation { get; set; }
        public string? ServiceCenterContact { get; set; }
    }
}
