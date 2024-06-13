namespace TP.Upgrade.Application.DTOs
{
    public class CurrencyLite
    {
        public short Id { get; set; }
        public string CurrencyCode { get; set; } 
        public string CountryCode { get; set; }
        public string Symbol { get; set; }
        public decimal Rate { get; set; }
    }
}
