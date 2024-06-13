using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class ProductSpecificationAttributeEntityTypeConfiguration : IEntityTypeConfiguration<ProductSpecificationAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductSpecificationAttribute> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .IsRequired();
            builder
                .HasOne(x => x.SpecificationAttribute)
                .WithMany()
                .HasForeignKey(x => x.SpecificationAttributeId)
                .IsRequired();
            builder
                .Property(x => x.AttributeTypeId)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .HasOne(x => x.SpecificationAttributeOption)
                .WithMany()
                .HasForeignKey(x => x.SpecificationAttributeOptionId)
                .IsRequired();
            #endregion
        }
    }
}
