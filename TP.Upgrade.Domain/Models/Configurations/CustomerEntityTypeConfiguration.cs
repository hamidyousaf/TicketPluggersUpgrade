using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.FirstName)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.LastName)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.Email)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.Username)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.PhoneNumber)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .IsRequired();
            builder
                .Property(x => x.AffiliateId)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .Property(x => x.IsVendor)
                .HasDefaultValue(false)
                .IsRequired();
            builder
                .Property(x => x.MaximumTicketSell)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .Property(x => x.VendorRequestStatus)
                .HasDefaultValue(0)
                .IsRequired();
            builder
                .Property(x => x.VendorAccountStatus)
                .HasDefaultValue(false)
                .IsRequired();
            builder
                .HasOne(x => x.BillingAddress)
                .WithOne()
                .HasForeignKey<Customer>(x => x.BillingAddressId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);
            builder
                .HasOne(x => x.ShippingAddress)
                .WithOne()
                .HasForeignKey<Customer>(x => x.ShippingAddressId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .HasOne(x => x.Currency)
                .WithOne()
                .HasForeignKey<Customer>(x => x.CurrencyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
            builder
                .HasOne(x => x.User)
                .WithOne()
                .IsRequired();
            builder
               .Property(x => x.CountryCode)
               .HasMaxLength(5)
               .HasDefaultValue("44")
               .IsRequired();
            builder
               .Property(x => x.IsDeleted)
               .HasDefaultValue(false)
               .IsRequired();
            builder
               .Property(x => x.IsActive)
               .HasDefaultValue(true)
               .IsRequired();
            builder
             .Property(b => b.CreatedDate)
             .HasDefaultValueSql("GETUTCDATE()")
             .IsRequired();
            #endregion

            #region Global query filter
            builder
                .HasQueryFilter(x => x.IsActive && !x.IsDeleted);
            #endregion
        }
    }
}