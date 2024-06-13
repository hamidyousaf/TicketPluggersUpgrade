namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetVendorAllProductSummaryRequest
    {
        public int vendorId { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public string searchText { get; set; } = string.Empty;
    }
}
