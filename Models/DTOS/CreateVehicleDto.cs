namespace VehicleServiceBook.Models.DTOS
{
    public class CreateVehicleDto
    {
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year {  get; set; }
        public string RegistrationNumber { get; set; } = null!;
    }
}
