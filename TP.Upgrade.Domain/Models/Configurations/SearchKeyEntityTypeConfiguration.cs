using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class SearchKeyEntityTypeConfiguration : IEntityTypeConfiguration<SearchKey>
    {
        public void Configure(EntityTypeBuilder<SearchKey> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.Keyword)
                .HasMaxLength(45)
                .IsRequired()
                .HasConversion(
                    x => x.ToString(),
                    x => x.Trim()
                );
            builder
                .Property(x => x.Count)
                .IsRequired();
            #endregion
        }
    }
}
