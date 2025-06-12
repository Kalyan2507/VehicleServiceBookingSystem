namespace VehicleServiceBook.Models.DTOS
{
    public class ProfileDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public int ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; }
        public string ServiceCenterLocation { get; set; }
        public string ServiceCenterContact { get; set; }
    }
}
