using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class PlatformFeeConfiguration : IEntityTypeConfiguration<PlatformFee>
    {
        public void Configure(EntityTypeBuilder<PlatformFee> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.UserTypeId)
                .IsRequired();
            builder
                .Property(x => x.UserTypeId)
                .IsRequired();
            builder
                .Property(x => x.PlatFormFeeType)
                .IsRequired();
            builder
                .Property(x => x.Amount)
                .IsRequired();
            builder
                .Property(x => x.AmountType)
                .IsRequired();
            builder
             .Property(b => b.CreatedDate)
             .HasDefaultValueSql("GETUTCDATE()")
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
