namespace VehicleServiceBook.Models.DTOS
{
    public class CreateBookingDto
    {
        public int? VehicleId { get; set; }

        public int? ServiceCenterId { get; set; }

        public DateTime Date { get; set; }

        public int? ServiceTypeId { get; set; }
        public string TimeSlot { get; set; } = null!;

        //public string? Status { get; set; }
    }
}
