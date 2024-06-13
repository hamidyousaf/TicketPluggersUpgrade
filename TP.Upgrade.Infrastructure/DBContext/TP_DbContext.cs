using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TP.Upgrade.Application.DTOs;
using TP.Upgrade.Domain.Models.Configurations;
using TP.Upgrade.Domain.Models.DBEntity;

namespace TP.Upgrade.Infrastructure.DBContext
{
    public class TP_DbContext : IdentityDbContext<User, Roles, string>
    {
        public TP_DbContext(DbContextOptions<TP_DbContext> options) : base(options)
        {
        }
        public DbSet<Event> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<EventStatus> EventStatuses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<CustomerFavourite> CustomerFavourites { get; set; }
        public DbSet<SearchKey> SearchKeys { get; set; }
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<SpecificationAttribute> SpecificationAttributes { get; set; }
        public DbSet<SpecificationAttributeOption> SpecificationAttributeOptions { get; set; }
        public DbSet<SplitTicketOption> SplitTicketOptions { get; set; }
        public DbSet<ProductSpecificationAttribute> ProductSpecificationAttributes { get; set; }
        public DbSet<ReportProblem> ReportProblems { get; set; }
        public DbSet<PlatformFee> PlatformFees{ get; set; }
        public DbSet<ProductDocument> ProductDocuments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region Apply entity configuration
            modelBuilder.ApplyConfiguration(new CustomerFavouriteEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AddressEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SearchKeyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerOrderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SpecificationAttributeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SpecificationAttributeOptionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SplitTicketOptionEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSpecificationAttributeEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ReportProblemEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new PlatformFeeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductDocumentConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new TaxConfiguration());
            #endregion

            #region Global query filter
            modelBuilder.Entity<Event>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Currency>().HasQueryFilter(x => !x.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(x => !x.IsDeleted && x.IsPublished);
            #endregion

            modelBuilder.Entity<GetOrdersByCustomerDto>().HasNoKey();
            modelBuilder.Entity<GetOrdersByCustomerDto>().ToTable(nameof(GetOrdersByCustomerDto), t =>
                t.ExcludeFromMigrations());
        }
    }
}