using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
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
                .Property(x => x.CustomerId)
                .HasColumnOrder(1);
            builder
                .Property(x => x.Address1)
                .HasColumnOrder(2)
                .HasMaxLength(1024)
                .IsRequired();
            builder
                .Property(x => x.Address2)
                .HasColumnOrder(3)
                .HasMaxLength(1024)
                .IsRequired(false);
            builder
                .Property(x => x.PostalCode)
                .HasColumnOrder(4)
                .HasMaxLength(11)
                .IsRequired();
            builder
                .Property(x => x.State)
                .HasColumnOrder(5)
                .HasMaxLength(35)
                .IsRequired();
            builder
                .Property(x => x.City)
                .HasColumnOrder(6)
                .HasMaxLength(35)
                .IsRequired();
            builder
                .Property(x => x.Country)
                .HasColumnOrder(7)
                .HasMaxLength(55)
                .IsRequired();
            builder
             .Property(b => b.CreatedDate)
                .HasColumnOrder(8)
             .HasDefaultValueSql("GETUTCDATE()")
             .IsRequired();
            builder
                .Property(x => x.UpdatedDate)
                .HasColumnOrder(9)
                .IsRequired(false);
            #endregion
        }
    }
}
