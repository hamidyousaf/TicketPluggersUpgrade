using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class ProductDocument : BaseEntity<long>
    {
        [MaxLength(500)]
        public string FileName { get; set; }
        [MaxLength(20)]
        public string FileExtension { get; set; }
        public int ProductId { get; set; }
        public long? OrderId { get; set; }
        public bool IsDownloaded { get; set; }
        public string ContentType { get; set; }
        public bool IsDeleted { get; set; }
        // navigational property.
        public Product Product { get; set; }
        public CustomerOrder? Order { get; set; }
    }
}
