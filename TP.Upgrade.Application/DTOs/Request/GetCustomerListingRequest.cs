namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetCustomerListingRequest
    {
        public long customerId { get; set; }
        public string sort { get; set; }
        public string searchText { get; set; }
        public string deliveryTypeList { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public string countryCode { get; set; }
        public int start { get; set; }
        public int rows { get; set; }
        public int tab { get; set; }
    }
}
