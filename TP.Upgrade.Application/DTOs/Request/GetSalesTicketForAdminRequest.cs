namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetSalesTicketForAdminRequest
    {
        public string SearchText { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public string FilterBy { get; set; }
        public int PageIndex { get; set; } = 0;
        public int _pageSize { get; set; } = 10;
        private const int MaxPageSize = 20;
        private const int MinPageSize = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : (value <= 0) ? MinPageSize : value;
        }
    }
}
