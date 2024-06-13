using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Currency : BaseEntity<short>
    {
        [Required, MaxLength(45)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(10)]
        public string CurrencyCode { get; set; } = string.Empty;
        [Required, MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;
        [Required, MaxLength(45)]
        public string Country { get; set; } = string.Empty;
        [Required, MaxLength(5)]
        public string CountryCode { get; set; } = string.Empty;
        [Required, Precision(18, 5)]
        public decimal Rate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
    }
}
