using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TP.Upgrade.Domain.Enums;

namespace TP.Upgrade.Application.DTOs
{
    public class GetOrderByIdDto
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
        public decimal OrderTotal { get; set; }
        public int? ShippingAddressId { get; set; }
        public DateTime CreatedDate { get; set;}
        public byte OrderStatusId { get; set; }
        public byte ShippingStatusId { get; set; }
        public decimal OrderIndividualPrice { get; set; }
        public DateTime? OrderConfirmDate { get; set; }
        public DateTime? DownloadedDate { get; set; }
        public CustomerForGetOrderByIdDto Customer { get; set; }
        public VendorForGetOrderByIdDto Vendor { get; set; }
        public ProductForGetOrderByIdDto Product { get; set; }
        public string ShippingStatus { get => SS.ToString(); }
        [NotMapped]
        public ShippingStatus SS
        {
            get => (ShippingStatus)ShippingStatusId;
            set => ShippingStatusId = (byte)value;
        }

        [NotMapped]
        public string OrderStatus
        {

            get => GetEnumDescription((OrderStatus)OrderStatusId);
            set => OrderStatus = value;
        }
        public static string GetEnumDescription(Enum e)
        {
            var fieldInfo = e.GetType().GetField(e.ToString());

            DescriptionAttribute[] enumAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (enumAttributes.Length > 0)
            {
                return enumAttributes[0].Description;
            }
            return e.ToString();
        }
    }
    public class CustomerForGetOrderByIdDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class ProductForGetOrderByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TicketRow { get; set; }
        public short CurrencyId { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        public byte TicketTypeId { get; set; }
        public EventForGetOrderByIdDto Event { get; set; }
        public string TicketType { get => TT.ToString(); }
        [NotMapped]
        public TicketType TT
        {
            get => (TicketType)TicketTypeId;
            set => TicketTypeId = (byte)value;
        }
    }
    public class VendorForGetOrderByIdDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class EventForGetOrderByIdDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime EventStartUTC { get; set; }
        public TimeSpan EventStartTime { get; set; }
        public byte EventStatusId { get; set; }
        public VenueForGetOrderByIdDto Venue { get; set; }
    }
    public class VenueForGetOrderByIdDto
    {
        public int Id { get; set; }
        public string VenueName { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
    }
}
