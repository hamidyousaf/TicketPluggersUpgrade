namespace TP.Upgrade.Application.DTOs
{
    public class GetOrderCountByStatusDto
    {
        public int Orders { get; set; }
        public int PendingOrders { get; set; }
        public int TotalSale { get; set; }
        public int PendingPayments { get; set; }
        public int Cancelled { get; set; }
    }
}
