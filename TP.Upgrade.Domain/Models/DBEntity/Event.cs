using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Event : BaseEntity<int>
    {
        public Event()
        {
            Event_ID = Guid.NewGuid().ToString();
        }
        [MaxLength(80)]
        public string Event_ID { get; set; }
        [Required, MaxLength(240)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string? Description { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string? Notes { get; set; } = string.Empty;
        [Required]
        public DateTime EventStartUTC { get; set; }
        [Required]
        public TimeSpan EventStartTime { get; set; }
        [Required]
        public DateTime EventEndUTC { get; set; }
        public DateTime? OnSaleStartDateTime { get; set; }
        public DateTime? OnSaleEndDateTime { get; set; }
        [Required, MaxLength(40)]
        public string TimeZone { get; set; }
        [Required]
        public byte EventStatusId { get; set; }
        [MaxLength(255)]
        public string? ImageURL { get; set; }
        [Required]
        public bool IsHotEvent { get; set; }
        [Required]
        public bool Published { get; set; }
        public int? DisplayOrder { get; set; } = 0;
        [Required]
        public short SegmentId { get; set; }
        [Required]
        public short GenreId { get; set; }
        [Required]
        public short SubGenreId { get; set; }
        public decimal? TicketMinPrice { get; set; }
        public decimal? TicketMaxPrice { get; set; }
        public int? TicketAvailable { get; set; }
        public bool? IsPaymentProcessed { get; set; }
        [Required]
        public int VenueId { get; set; }
        public int? TicketUploaded { get; set; }
        public bool? IsFeatured { get; set; }
        public int? EventSearchCount { get; set; } = 0;
        public string? SearchName { get; set; }
        public bool? IsCustom { get; set; }
        public bool IsDeleted { get; set; }
        [Required, MaxLength(10)]
        public string CurrencyCode { get; set; }
        // navigational property
        public virtual Category Segment { get; set; }
        public virtual Category Genre { get; set; }
        public virtual Category SubGenre { get; set; }
        [ForeignKey(nameof(EventStatusId))]
        public virtual EventStatus EventStatus { get; set; }
        [ForeignKey(nameof(VenueId))]
        public virtual Venue Venue { get; set; }
    }
}