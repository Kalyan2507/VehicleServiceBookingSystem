namespace VehicleServiceBook.Models.DTOS
{
    public class VehicleDto
    {
        public int Vehicleid { get; set; }
        public string Make { get; set; } = null!;
        public string Model { get; set; } = null!;
        public int Year {  get; set; }
        public string RegistrationNumber { get; set; } = null!;
    }
}
