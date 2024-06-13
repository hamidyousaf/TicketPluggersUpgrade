namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class Tax
    {
        public short Id { get; set; }
        public string TaxName { get; set; }
        public string TaxType { get; set; }
        public decimal TaxFee { get; set; }
        public string CountryCode { get; set; }
        public string UserGroup { get; set; }
        public short Priority { get; set; }
        public bool IsDeleted { get; set; }
    }
}
