namespace VehicleServiceBook.Models.DTOS
{
    public class AppointmentDto
    {
        public string RegistrationNumber { get; set; }
        public string CustomerName { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Pending";

    }
}
