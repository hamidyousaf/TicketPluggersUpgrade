using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.NotificationDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
            builder
                .Property(x => x.NotificationContent)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.FromUserId)
                .IsRequired();
            builder
                .Property(x => x.ToUserId)
                .IsRequired();
            builder
                .Property(x => x.Subject)
                .HasDefaultValue("")
                .IsRequired(false);
            builder
             .Property(b => b.CreatedDate)
             .HasDefaultValueSql("GETUTCDATE()")
             .IsRequired();
            builder
             .Property(b => b.IsDeleted)
             .HasDefaultValue(false)
             .IsRequired();
            builder
             .Property(b => b.IsRead)
             .HasDefaultValue(false)
             .IsRequired();
            builder
             .Property(b => b.OrderId)
             .IsRequired();
            #endregion

            #region Global query filter
            builder
                .HasQueryFilter(x => !x.IsDeleted);
            #endregion
        }
    }
}
