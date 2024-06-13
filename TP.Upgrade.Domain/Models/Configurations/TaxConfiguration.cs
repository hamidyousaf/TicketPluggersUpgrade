using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class TaxConfiguration : IEntityTypeConfiguration<Tax>
    {
        public void Configure(EntityTypeBuilder<Tax> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.TaxName)
                .HasMaxLength(100)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.TaxType)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.TaxFee)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder
                .Property(x => x.CountryCode)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.UserGroup)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.Priority)
                .IsRequired();
            builder
             .Property(b => b.IsDeleted)
             .HasDefaultValue(false)
             .IsRequired();
            #endregion

            #region Global query filter
            builder
                .HasQueryFilter(x => !x.IsDeleted);
            #endregion
        }
    }
}
