namespace VehicleServiceBook.Models.DTOS
{
    public class BookingDto
    {
        public int? BookingId { get; set; }
        public int VehicleId { get; set; }

        public string Make { get; set; }    
        public string? RegistrationNumber { get; set; }
        public int? ServiceCenterId { get; set; }

        public string ServiceCenterName { get; set; }
        public int? ServiceTypeId { get; set; }

        public string ServiceTypeDescription { get; set; }
        public string MechanicName { get; set; }
        public string TimeSlot { get; set; }
        public DateTime Date { get; set; }
        public string? ServiceStatus { get; set; }




    }
}
