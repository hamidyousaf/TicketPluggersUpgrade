using System.Diagnostics.Metrics;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.DTOs
{
    public class GetVendorAllProductSummaryDto
    {
        public string? City { get; set; } = string.Empty;
        public string? Country { get; set; } = string.Empty;
        public DateTime EventStart { get; set; }
        public TimeSpan StartTime { get; set; }
        public long Id { get; set; }
        public string? Venue { get; set; } = string.Empty;
        public long VenueId { get; set; }
        public string? Name { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
    }
}