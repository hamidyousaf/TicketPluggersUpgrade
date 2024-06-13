using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Product : BaseEntity<int>
    {
        [Required]
        public int EventId { get; set; }
        [Required, MaxLength(255)]
        public string Name { get; set; } = string.Empty;
        public long VendorId { get; set; }
        [Required]
        public int SoldQuantity { get; set; }
        [Required]
        public int StockQuantity { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal TotalPrice { get; set; }
        [Required]
        public string CurrencySymbol { get; set; } = string.Empty;
        [Required]
        public string CurrencyCode { get; set; } = string.Empty;
        [Required]
        public short CurrencyId { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public decimal PriceInPound { get; set; }
        [Required]
        public decimal ProceedCost { get; set; }
        [Required]
        public decimal FaceValue { get; set; }
        [Required]
        public int TicketRow { get; set; }
        [Required]
        public int SeatsFrom { get; set; }
        [Required]
        public int SeatsTo { get; set; }
        public byte TicketTypeId { get; set; }
        public byte TicketSpliting { get; set; }
        public int SectionId { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? haveTicketDateContent { get; set; }
        public int? BillingAddressId { get; set; }
        public bool IsInstantDelivery { get; set; }
        [Required]
        public byte SplitTicketOptionId { get; set; }
        public string? ProofCertificate { get; set; }
        public string? Notes { get; set; }
        public string? TransferRecipt { get; set; }
        public string? TicketLinks { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        // navigational property
        [ForeignKey(nameof(CurrencyId))]
        public virtual Currency Currency { get; set; }
        [ForeignKey(nameof(EventId))]
        public virtual Event Event { get; set; }
        [ForeignKey(nameof(VendorId))]
        public virtual Customer Vendor { get; set; }
        [ForeignKey(nameof(SplitTicketOptionId))]
        public virtual SplitTicketOption SplitTicketOption { get; set; }
        public ICollection<ProductDocument> ProductDocuments { get; set; }
    }
}
