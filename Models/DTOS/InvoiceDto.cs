namespace VehicleServiceBook.Models.DTOS
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }

        public int BookingId { get; set; }

        public int? ServiceTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; } = null;

        public DateTime PaymentDate { get; set; }
        public string ServiceTypeDescription { get; set; }
        public DateTime? BookingDate { get; set; }
        public string BookingStatus { get; set; }
    }
}
