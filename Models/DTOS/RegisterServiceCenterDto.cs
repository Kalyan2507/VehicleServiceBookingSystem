namespace VehicleServiceBook.Models.DTOS
{
    public class RegisterServiceCenterDto
    {
        public RegisterUserDto User { get; set; } = null!;
        public string? ServiceCenterName { get; set; }

        public string? ServiceCenterLocation { get; set; }

        public string? ServiceCenterContact { get; set; }
        //public object User { get; internal set; }
    }
}
