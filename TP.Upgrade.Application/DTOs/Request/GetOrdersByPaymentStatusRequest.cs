namespace TP.Upgrade.Application.DTOs.Request
{
    public class GetOrdersByPaymentStatusRequest
    {
        public int Tab { get; set; }
        public string SearchString { get; set; } = string.Empty;
    }
}
