using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class ChangePasswordRequest
    {
        public string UserId { get; set; }
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
