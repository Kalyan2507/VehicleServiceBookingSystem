namespace VehicleServiceBook.Models.DTOS
{
    public class CreateServiceTypeDto
    {
        public string Description {  get; set; } = null!;
        public decimal Price {  get; set; }
    }
}
