using System.ComponentModel.DataAnnotations.Schema;

namespace TP.Upgrade.Application.DTOs
{
    public class ListingWrapper : Listing
    {
        public decimal Commission { get; set; }
        public string EventName { get; set; }
        public DateTime EventStart { get; set; }
        public string StartTime { get; set; }
        public long VenueId { get; set; }
        public long VendorId { get; set; }
        public string Venue { get; set; }
        public string VenueCountry { get; set; }
        public string VenueCity { get; set; }
        public string VendorFirstName { get; set; }
        public string VendorLastName { get; set; }
        public bool IsTicketUploaded { get; set; }
        [NotMapped]
        public bool IsExpired { get; set; }
    }
    public class Listing
    {
        public long Id { get; set; }
        public int SectionId { get; set; }
        public string Name { get; set; }
        public long eventId { get; set; }
        public string ticketSeperation { get; set; }
        public int seatsTo { get; set; }
        public int seatsFrom { get; set; }
        public string Currency { get; set; }
        public string CurrencyCode { get; set; }
        public bool IsInstantDelivery { get; set; }
        public long VendorId { get; set; }
        public bool IsMyListing { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public decimal SellerFees { get; set; }
        public decimal Willrecive { get; set; }
        public decimal PriceInPound { get; set; }
        public int ticketRow { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public int OrderMaximumQuantity { get; set; }
        public string AllowedQuantities { get; set; }
        public bool Published { get; set; }
        public bool Deleted { get; set; }
        public int ticketTypeId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? haveTicketDateContent { get; set; }
        public decimal tax { get; set; }
        public decimal platFormFee { get; set; }
        public int platformFeeType { get; set; }
        public int taxtype { get; set; }
        public IList<byte> SelectedSpecificationAttributeIds { get; set; }
        public int TicketSpliting { get; set; }
    }
    public class EventProduct
    {
        public decimal Commission { get; set; }
        public string EventName { get; set; }
        public DateTime EventStart { get; set; }
        public string StartTime { get; set; }
        public long VenueId { get; set; }
        public string Venue { get; set; }
        public string VenueCountry { get; set; }
        public string VenueCity { get; set; }
        public string VendorFirstName { get; set; }
        public string VendorLastName { get; set; }
        public long VendorId { get; set; }
        public int TicketCount { get; set; }
        public int TotalSoldQuantity { get; set; }
        public int TotalRemainingQuantity { get; set; }
        public bool AllPublished { get; set; }
        public bool IsExpired { get; set; }
        public bool IsTicketUploaded { get; set; }
        public List<Listing> Tickets { get; set; }
    }
}
