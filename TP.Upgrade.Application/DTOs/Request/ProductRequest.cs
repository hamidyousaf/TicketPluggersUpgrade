using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class CreateProductRequest
    {
        [Required]
        public int EventId { get; set; }
        public long VendorId { get; set; } = 0;
        [Required]
        public int TicketTypeId { get; set; }
        public string ShortDescription { get; set; } = string.Empty;
        [Required]
        public byte SplitTicketOptionId { get; set; }
        [Required]
        public int SectionId { get; set; }
        [Required]
        public int TicketRow { get; set; }
        [Required]
        public int SeatsFrom { get; set; }
        [Required]
        public int SeatsTo { get; set; }
        public IList<byte> SelectedSpecificationAttributeIds { get; set; }
        public bool IsInstantDelivery { get; set; }
        public DateTime? haveTicketDateContent { get; set; }
        [JsonPropertyName("File")]
        public IList<IFormFile>? Files { get; set; }
        [Required]
        public short CurrencyId { get; set; }
        [Required]
        public decimal FaceValue { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal ProceedCost { get; set; }
    }
    public class UpdateProductRequest : CreateProductRequest
    {
        public int Id { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
    }
}