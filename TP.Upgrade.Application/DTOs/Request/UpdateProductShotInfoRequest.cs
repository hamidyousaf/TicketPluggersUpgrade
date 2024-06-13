namespace TP.Upgrade.Application.DTOs.Request
{
    public class UpdateProductShotInfoRequest
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public decimal ProceedCost { get; set; }
        public decimal FaceValue { get; set; }
        public int StockQuantity { get; set; }
        public string Notes { get; set; }
    }
}
