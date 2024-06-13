using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Application.DTOs
{
    public class GetCustomerProfileByIdDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public GetCustomerProfileByIdVM? BillingAddress { get; set; }
        public bool VendorAccountStatus { get; set; }
    }
    public class GetCustomerProfileByIdVM
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}
