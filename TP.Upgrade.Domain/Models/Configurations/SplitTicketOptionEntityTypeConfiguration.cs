using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class SplitTicketOptionEntityTypeConfiguration : IEntityTypeConfiguration<SplitTicketOption>
    {
        public void Configure(EntityTypeBuilder<SplitTicketOption> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.Name)
                .HasMaxLength(55)
                .IsRequired();
            builder
                .Property(x => x.DisplayOrder)
                .IsRequired();
            #endregion
        }
    }
}
