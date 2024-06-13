using System.ComponentModel.DataAnnotations;

namespace TP.Upgrade.Application.DTOs.Request
{
    public class UpdateUserRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
