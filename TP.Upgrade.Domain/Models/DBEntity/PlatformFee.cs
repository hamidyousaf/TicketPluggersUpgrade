namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class PlatformFee : BaseEntity<short>
    {
        public int UserTypeId { get; set; }
        public int PlatFormFeeType { get; set; }
        public decimal Amount { get; set; }
        public byte AmountType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
