using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class EmployeeRegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        public long VendorId { get; set; }
    }
}
