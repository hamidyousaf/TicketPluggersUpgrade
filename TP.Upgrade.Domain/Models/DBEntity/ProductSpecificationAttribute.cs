namespace TP.Upgrade.Domain.Models.DBEntity
{
    public class ProductSpecificationAttribute
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public byte AttributeTypeId { get; set; }
        public byte SpecificationAttributeId { get; set; }
        public byte SpecificationAttributeOptionId { get; set; }
        // navigation property
        public Product Product { get; set; }
        public SpecificationAttribute SpecificationAttribute { get; set; }
        public SpecificationAttributeOption SpecificationAttributeOption { get; set; }
    }
}
