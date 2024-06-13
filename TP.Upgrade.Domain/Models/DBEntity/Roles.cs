using Microsoft.AspNetCore.Identity;
using TP.Upgrade.Domain.Enums;

namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Roles : IdentityRole
    {
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
