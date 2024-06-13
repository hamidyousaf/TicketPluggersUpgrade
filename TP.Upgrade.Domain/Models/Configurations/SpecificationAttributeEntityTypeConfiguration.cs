using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class SpecificationAttributeEntityTypeConfiguration : IEntityTypeConfiguration<SpecificationAttribute>
    {
        public void Configure(EntityTypeBuilder<SpecificationAttribute> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.Name)
                .HasMaxLength(45)
                .IsRequired();
            builder
                .Property(x => x.DisplayOrder)
                .IsRequired();
            #endregion
        }
    }
}
