namespace VehicleServiceBook.Models.DTOS
{
    public class ServiceTypeDto
    {
        public int ServiceTypeid {  get; set; }
        public string Description { get; set; } = null!;
        public decimal Price {  get; set; }
    }
}
