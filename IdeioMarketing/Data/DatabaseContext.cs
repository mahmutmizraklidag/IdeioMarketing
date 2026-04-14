using IdeioMarketing.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace IdeioMarketing.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<SubPackage> SubPackages { get; set; }
        public DbSet<PackageFeatures> PackageFeatures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PaymentPlan> PaymentPlans { get; set; }
        public DbSet<OfferRecord> OfferRecords { get; set; }
        public DbSet<OfferRecordItem> OfferRecordItems { get; set; }
        public DbSet<OfferRecordInstallment> OfferRecordInstallments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OfferRecord>(entity =>
            {
                entity.Property(x => x.GrossTotal).HasColumnType("decimal(18,2)");
                entity.Property(x => x.NetTotal).HasColumnType("decimal(18,2)");
                entity.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
                entity.Property(x => x.DiscountRate).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<OfferRecordItem>(entity =>
            {
                entity.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");

                entity.HasOne(x => x.OfferRecord)
                    .WithMany(x => x.Items)
                    .HasForeignKey(x => x.OfferRecordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OfferRecordInstallment>(entity =>
            {
                entity.Property(x => x.GrossAmount).HasColumnType("decimal(18,2)");
                entity.Property(x => x.NetAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(x => x.OfferRecord)
                    .WithMany(x => x.Installments)
                    .HasForeignKey(x => x.OfferRecordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                password = "xyz123456"
            });
        }

    }
}
