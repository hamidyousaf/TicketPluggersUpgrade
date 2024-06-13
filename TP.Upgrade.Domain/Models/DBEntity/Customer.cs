using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Customer
    {
        [Key]
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public long AffiliateId { get; set; }
        public bool IsVendor { get; set; }
        public int MaximumTicketSell { get; set; }
        public byte VendorRequestStatus { get; set; }
        public bool VendorAccountStatus { get; set; }
        public short? CurrencyId { get; set; }
        public string UserId { get; set; }
        public string CountryCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? BillingAddressId { get; set; }
        public int? ShippingAddressId { get; set; }
        public Address? BillingAddress { get; set; }
        public Address? ShippingAddress { get; set; }
        public Currency? Currency { get; set; }
        public User User { get; set; }
    }
}