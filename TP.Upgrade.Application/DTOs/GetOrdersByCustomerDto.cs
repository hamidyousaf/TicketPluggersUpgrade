using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using TP.Upgrade.Domain.Enums;

namespace TP.Upgrade.Application.DTOs
{
    public class GetOrdersByCustomerDto
    {
        public long OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? OrderConfirmDate { get; set; }
        public string SectionName { get; set; }
        public int SectionRow { get; set; }
        public string EventName { get; set; }
        public byte? EventStatus { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public int? Quantity { get; set; }
        public decimal? OrderTotal { get; set; }
        public decimal? OrderIndividualPrice { get; set; }
        public long? CustomerID { get; set; }
        public int ProductId { get; set; }
        public string CustomerFName { get; set; }
        public short CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string CustomerLName { get; set; }
        public byte OrderStatusId { get; set; }
        public byte ShippingStatusId { get; set; }
        public byte ticketType { get; set; }
        public string? PaymentId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string VenueName { get; set; }
        public string VenueCity { get; set; }
        public string VenueCountry { get; set; }
        [NotMapped]
        public OrderStatus OS
        {
            get => (OrderStatus)OrderStatusId;
            set => OrderStatusId = (byte)value;
        }
        [NotMapped]
        public ShippingStatus SS
        {
            get => (ShippingStatus)ShippingStatusId;
            set => ShippingStatusId = (byte)value;
        }
        [NotMapped]
        public TicketType TT
        {
            get => (TicketType)ticketType;
            set => ticketType = (byte)value;
        }
        public string OrderStatus { get => OS.ToString(); }
        public string ShippingStatus { get => SS.ToString(); }
        public string ProductType { get => TT.ToString(); }
    }
}
