using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP.Upgrade.Application.DTOs
{
    public class ProductLite
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public long VendorId { get; set; }
        public int SoldQuantity { get; set; }
        public int StockQuantity { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencyCode { get; set; }
        public short CurrencyId { get; set; }
        public decimal Price { get; set; }
        public decimal PriceInPound { get; set; }
        public decimal ProceedCost { get; set; }
        public decimal FaceValue { get; set; }
        public int TicketRow { get; set; }
        public int SeatsFrom { get; set; }
        public int SeatsTo { get; set; }
        public byte TicketTypeId { get; set; }
        public int SectionId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? haveTicketDateContent { get; set; }
        public int? BillingAddressId { get; set; }
        public bool IsInstantDelivery { get; set; }
        public byte SplitTicketOptionId { get; set; }
        public string? ProofCertificate { get; set; }
        public string? Notes { get; set; }
        public string? TransferRecipt { get; set; }
        public string? TicketLinks { get; set; }
        public string ShortDescription { get; set; }
    }
    public class ProductLiteWithSpecifications : ProductLite
    {
        public List<byte> SelectedSpecificationAttributeIds { get; set; }
        public List<byte> TicketFeatures { get; set; }
        public List<byte> TicketDiscloure { get; set; }
    }
    public class GetProductByIdForEditForAppDto : ProductLiteWithSpecifications
    {
        public string EventName { get; set; }
        public DateTime EventStart { get; set; }
        public string Venue { get; set; }
        public string VenueCity { get; set; }
        public string VenueCountry { get; set; }
        public string StartTime { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Willrecive { get; set; }
        public decimal SellerFees { get; set; }
    }
}
