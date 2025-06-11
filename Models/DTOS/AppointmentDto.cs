namespace VehicleServiceBook.Models.DTOS
{
    public class AppointmentDto
    {
        public string RegistrationNumber { get; set; }
        public string CustomerName { get; set; }
        public string MechanicName { get; set; }
        public string ServiceTypeDescription { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; }
        public decimal Price { get; set; }
        public string ServiceStatus { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Pending";

    }
}
