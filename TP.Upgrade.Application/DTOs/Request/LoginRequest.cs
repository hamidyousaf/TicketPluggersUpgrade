using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class LoginRequest
    {
        [StringLength(50)]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;
        [StringLength(20)]
        public string? MobileNo { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Password { get; set; }
    }
}
