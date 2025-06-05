namespace VehicleServiceBook.Models.DTOS
{
    public class MechanicDto
    {
        public int MechanicId { get; set; }
        public int ServiceCenterId { get; set; }
        public string MechanicName { get; set; } = null!;
        public string Expertise { get; set; } = null!;
    }
}
