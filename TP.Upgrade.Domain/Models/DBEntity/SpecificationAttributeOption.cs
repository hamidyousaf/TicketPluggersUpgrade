namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class SpecificationAttributeOption
    {
        public byte Id { get; set; }
        public byte SpecificationAttributeId { get; set; }
        public string Name { get; set; }
        public byte DisplayOrder { get; set; }
        // navigational property
        public SpecificationAttribute SpecificationAttribute { get; set; }
    }
}
