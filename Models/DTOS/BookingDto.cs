namespace VehicleServiceBook.Models.DTOS
{
    public class BookingDto
    {
        public int? BookingId { get; set; }
        public int VehicleId { get; set; }
        public string? RegistrationNumber { get; set; }
        public int? ServiceCenterId { get; set; }
        public int? ServiceTypeId { get; set; }

        public string Description { get; set; }
        public string MechanicName { get; set; }
        public string TimeSlot { get; set; }
        public DateTime Date { get; set; }
        public string? Status { get; set; }




    }
}
