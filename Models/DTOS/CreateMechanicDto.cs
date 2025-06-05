namespace VehicleServiceBook.Models.DTOS
{
    public class CreateMechanicDto
    {
        public int ServiceCenterId { get; set; }
        public string MechanicName { get; set; } = null!;
        public string Expertise { get; set; } = null!;
    }
}
