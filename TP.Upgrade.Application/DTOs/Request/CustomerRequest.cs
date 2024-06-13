using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class CreateCustomerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [NotMapped]
        public string UserId { get; set; }
    }
    public class UpdateCustomerRequest
    {
        public long CustomerId { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }
        public UpdateCustomerRequestForAddressVM Address { get; set; }
        public UpdateCustomerRequestForPasswordVM Password { get; set; }
    }
    public class UpdateCustomerRequestForAddressVM
    {
        [Required(AllowEmptyStrings = false)]
        public string Address1 { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string PostalCode { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string State { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string City { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string Country { get; set; }
    }
    public class UpdateCustomerRequestForPasswordVM
    {
        [Required(AllowEmptyStrings = false)]
        public string CurrentPassword { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 5)]
        public string NewPassword { get; set; }
        [Compare("NewPassword")]
        [Required(AllowEmptyStrings = false)]
        [StringLength(50, MinimumLength = 5)]
        public string ConfirmPassword { get; set; }
    }
}
