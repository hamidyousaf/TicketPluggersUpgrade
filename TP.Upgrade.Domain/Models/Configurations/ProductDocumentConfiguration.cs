using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Domain.Models.Configurations
{
    public class ProductDocumentConfiguration : IEntityTypeConfiguration<ProductDocument>
    {
        public void Configure(EntityTypeBuilder<ProductDocument> builder)
        {
            #region Entity configuration
            builder
                .HasKey(x => x.Id);
            builder
                .Property(x => x.FileName)
                .HasDefaultValue("")
                .HasMaxLength(500)
                .IsRequired();
            builder
                .Property(x => x.FileExtension)
                .HasDefaultValue("")
                .HasMaxLength(20)
                .IsRequired();
            builder
                .Property(x => x.ContentType)
                .HasDefaultValue("")
                .HasMaxLength(100)
                .IsRequired();
            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.ProductDocuments)
                .HasForeignKey(x => x.ProductId)
                .IsRequired();
            builder 
                .HasOne(x => x.Order)
                .WithOne()
                .HasForeignKey<ProductDocument>(x => x.OrderId)
                .IsRequired(false);
            builder
             .Property(b => b.IsDownloaded)
             .HasDefaultValue(false)
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
