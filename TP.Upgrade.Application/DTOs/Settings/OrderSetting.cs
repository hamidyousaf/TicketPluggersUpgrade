namespace TP.Upgrade.Application.DTOs.Settings
{
    public class OrderSetting
    {
        public int MinimumOrderPlacementInterval { get; set; }
        public bool AttachPdfInvoiceToOrderPaidEmail { get; set; }
        public decimal MinOrderTotalAmount { get; set; }
        public decimal DefaultOrderCommision { get; set; }
    }
}
