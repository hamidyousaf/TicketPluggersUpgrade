namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetOrdersByCustomerRequest
    {
        public string SearchText { get; set; } = string.Empty;
        public long CustomerId { get; set; }
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 20;
        public int Tab { get; set; }
    }
}
