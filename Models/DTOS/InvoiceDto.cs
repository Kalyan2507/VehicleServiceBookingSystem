namespace VehicleServiceBook.Models.DTOS
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }

        public int BookingId { get; set; }

        public int? ServiceTypeId { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentStatus { get; set; } = null;
    }
}
