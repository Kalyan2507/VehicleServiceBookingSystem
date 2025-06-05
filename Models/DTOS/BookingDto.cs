namespace VehicleServiceBook.Models.DTOS
{
    public class BookingDto
    {
        public int? BookingId { get; set; }
        public string TimeSlot { get; set; } = null!;
        public DateTime Date { get; set; }
        public string? Status { get; set; }
        public int VehicleId {  get; set; }

        public int? ServiceCenterId { get; set; }

        

        
    }
}
