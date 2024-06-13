namespace TP.Upgrade.Application.DTOs
{
    public class CustomerOrderLite
    {
        public long CustomerId { get; set; }
        public long VendorId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal OrderIndividualPrice { get; set; }
        public decimal OrderTotal { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        public byte OrderStatusId { get; set; }
        public byte PaymentStatusId { get; set; }
        public byte ShippingStatusId { get; set; }
        public byte TicketTypeId { get; set; }
        public int BillingAddressId { get; set; }
        public string PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal AmountPayedToVendor { get; set; }
        public DateTime? OrderConfirmDate { get; set; } // When vendor confirmed order.
        public DateTime? UploadETicketDate { get; set; } // When vendor upload ticket.
        public DateTime? DownloadedDate { get; set; } // When customer download ticket.
        public DateTime? ExpectedTicketUploadDate { get; set; }
        public DateTime? GetPaidRequestDate { get; set; } // When vendor requested for payment to admin.
        public DateTime? ExpectShippingDate { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? ResetShipmentRequestDate { get; set; }
        public string ResetShipmentReason { get; set; }
    }
}
