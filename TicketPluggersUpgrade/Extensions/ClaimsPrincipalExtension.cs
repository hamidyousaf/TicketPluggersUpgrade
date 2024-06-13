using System.Security.Claims;

namespace TP.Upgrade.Api.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(x => x.Type == "Email")?.Value;
        }
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        public static long GetCustomerId(this ClaimsPrincipal user)
        {
            var customerId = user.FindFirst(x => x.Type == "CustomerId")?.Value;
            return customerId is null ? 0 : long.Parse(customerId);
        }
    }
}
