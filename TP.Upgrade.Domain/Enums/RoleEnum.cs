using System.ComponentModel;

namespace TP.Upgrade.Domain.Enums
{
    public enum RoleEnum
    {
        [Description("SuperAdmin")]
        SuperAdmin = 1,
        [Description("Advance Seller")]
        AdvanceSeller = 2,
        [Description("User")]
        NoramlUser = 3,
    }
}
