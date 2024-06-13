using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class CreateAddressRequest
    {
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public long CustomerId { get; set; }
    }
    public class UpdateAddressRequest : CreateAddressRequest
    {
        public int Id { get; set; }
        public string UpdatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedDate { get; set; }
    }
    public class DeleteAddressRequest
    {
        [Required]
        public int Id { get; set; }
        public long CustomerId { get; set; }
    }
}
