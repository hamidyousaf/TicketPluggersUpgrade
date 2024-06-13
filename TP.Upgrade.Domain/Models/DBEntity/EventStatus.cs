using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class EventStatus : BaseEntity<byte>
    {
        [Required, MaxLength(40)]
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}
