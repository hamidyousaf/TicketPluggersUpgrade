namespace TP.Upgrade.Application.DTOs
{
    public class GetCustomerSimpleProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CountryCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string? ProfilePicture { get; set; }
    }
}
