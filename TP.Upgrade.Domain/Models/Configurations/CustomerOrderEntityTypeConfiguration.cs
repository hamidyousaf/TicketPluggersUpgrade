using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class CustomerOrderEntityTypeConfiguration : IEntityTypeConfiguration<CustomerOrder>
    {
        public void Configure(EntityTypeBuilder<CustomerOrder> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .IsRequired();
            builder
                .HasOne(x => x.Vendor)
                .WithMany()
                .HasForeignKey(x => x.VendorId)
                .IsRequired();
            builder
                .HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .IsRequired();
            builder
                .Property(x => x.Quantity)
                .IsRequired();
            builder
                .Property(x => x.OrderIndividualPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder
                .Property(x => x.OrderTotal)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
            builder
                .Property(x => x.CurrencySymbol)
                .IsRequired();
            builder
                .Property(x => x.CurrencyCode)
                .IsRequired();
            builder
                .Property(x => x.OrderStatusId)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .Property(x => x.PaymentStatusId)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .Property(x => x.ShippingStatusId)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .Property(x => x.TicketTypeId)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .Property(x => x.PaymentId)
                .IsRequired(false);
            builder
                .Property(x => x.TimeExtensionRequestStatus)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .Property(x => x.ResetShipmentReason)
                .IsRequired(false);
            builder
                .Property(x => x.AmountPayedToVendor)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0.00)
                .IsRequired();
            builder
                .HasOne(x => x.BillingAddress)
                .WithMany()
                .HasForeignKey(x => x.BillingAddressId)
                .IsRequired();
            builder
                .HasOne(x => x.ShippingAddress)
                .WithOne()
                .HasForeignKey<CustomerOrder>(x => x.ShippingAddressId)
                .IsRequired(false);
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
