using System.Security.Cryptography;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Address
    {
        public int Id { get; set; }
        public long CustomerId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        // navigational property
        public Customer Customer { get; set; }
    }
}
