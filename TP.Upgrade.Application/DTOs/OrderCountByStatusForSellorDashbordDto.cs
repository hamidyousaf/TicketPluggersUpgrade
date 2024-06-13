namespace TP.Upgrade.Application.DTOs
{
    public class OrderCountByStatusForSellorDashbordDto
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int TotalSale { get; set; }
        public int BadOrder { get; set; }
        public int PendingPayments { get; set; }
        public int Cancelled { get; set; }
        public decimal Proceed { get; set; }
        public decimal FailurRate { get; set; }
        public int LateShipment { get; set; }
    }
}
