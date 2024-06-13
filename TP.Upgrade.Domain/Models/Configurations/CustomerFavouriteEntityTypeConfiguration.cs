using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class CustomerFavouriteEntityTypeConfiguration : IEntityTypeConfiguration<CustomerFavourite>
    {
        public void Configure(EntityTypeBuilder<CustomerFavourite> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(x => x.Event)
                .WithMany()
                .HasForeignKey(x => x.EventId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasOne(x => x.Venue)
                .WithMany()
                .HasForeignKey(x => x.VenueId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
            builder
                .Property(x => x.PerformerId)
                .IsRequired(false);
            builder
                .Property(x => x.FavouriteType)
                .IsRequired();
            builder
                .Property(x => x.NotifyStatus)
                .IsRequired();
            builder
                .Property(x => x.IsFavourite)
                .IsRequired();
            builder
                .Property(b => b.CreatedDate)
                .HasDefaultValueSql("GETUTCDATE()")
                .IsRequired();
            #endregion

            #region Global query filter
            builder
                .HasQueryFilter(x => x.IsFavourite);
            #endregion
        }
    }
}
