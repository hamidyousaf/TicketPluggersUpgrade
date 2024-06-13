using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class ReportProblemEntityTypeConfiguration : IEntityTypeConfiguration<ReportProblem>
    {
        public void Configure(EntityTypeBuilder<ReportProblem> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .Property(x => x.IsCustomerSend)
                .HasDefaultValue(false)
                .IsRequired();
            builder
                .HasOne(x => x.Event)
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .Property(x => x.PaymentId)
                .HasMaxLength(20)
                .IsRequired(false);
            builder
                .Property(x => x.ChatType)
                .IsRequired();
            builder
                .Property(x => x.Message)
                .IsRequired();
            builder
                .Property(x => x.ReferenceLink)
                .IsRequired(false);
            builder
                .Property(x => x.ReferenceFile)
                .IsRequired(false);
            builder
                .Property(b => b.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
            builder
               .Property(x => x.IsDeleted)
               .HasDefaultValue(false)
               .IsRequired();
            builder
               .Property(x => x.IsActive)
               .HasDefaultValue(true)
               .IsRequired();
            #endregion

            #region Global query filter
            builder
                .HasQueryFilter(x => x.IsActive && !x.IsDeleted);
            #endregion
        }
    }
}
