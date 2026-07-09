using IdeioMarketing.MarketingFeature.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdeioMarketing.MarketingFeature.Data
{
    public class MarketingDatabaseContext : DbContext
    {
        public MarketingDatabaseContext(DbContextOptions<MarketingDatabaseContext> options) : base(options)
        {
        }

        public DbSet<MarketingLead> MarketingLeads { get; set; }
        public DbSet<MarketingLeadOwner> MarketingLeadOwners { get; set; }
        public DbSet<MarketingOwner> MarketingOwners { get; set; }
        public DbSet<MarketingStage> MarketingStages { get; set; }
        public DbSet<MarketingSource> MarketingSources { get; set; }
        public DbSet<MarketingLeadStatus> MarketingLeadStatuses { get; set; }
        public DbSet<MarketingLeadTemperature> MarketingLeadTemperatures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MarketingLead>(entity =>
            {
                entity.Property(x => x.Value).HasColumnType("decimal(18,2)");
                entity.HasIndex(x => x.ExternalId).IsUnique();

                entity.HasOne(x => x.Source)
                    .WithMany(x => x.Leads)
                    .HasForeignKey(x => x.SourceId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Status)
                    .WithMany(x => x.Leads)
                    .HasForeignKey(x => x.StatusId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Temperature)
                    .WithMany(x => x.Leads)
                    .HasForeignKey(x => x.TemperatureId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Stage)
                    .WithMany(x => x.Leads)
                    .HasForeignKey(x => x.StageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MarketingLeadOwner>(entity =>
            {
                entity.HasKey(x => new { x.MarketingLeadId, x.MarketingOwnerId });

                entity.HasOne(x => x.MarketingLead)
                    .WithMany(x => x.LeadOwners)
                    .HasForeignKey(x => x.MarketingLeadId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.MarketingOwner)
                    .WithMany(x => x.LeadOwners)
                    .HasForeignKey(x => x.MarketingOwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MarketingStage>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<MarketingSource>().HasIndex(x => x.Name).IsUnique();
            modelBuilder.Entity<MarketingLeadStatus>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<MarketingLeadTemperature>().HasIndex(x => x.Key).IsUnique();
            modelBuilder.Entity<MarketingOwner>().HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<MarketingStage>().HasData(MarketingSeedData.Stages);
            modelBuilder.Entity<MarketingSource>().HasData(MarketingSeedData.Sources);
            modelBuilder.Entity<MarketingLeadStatus>().HasData(MarketingSeedData.Statuses);
            modelBuilder.Entity<MarketingLeadTemperature>().HasData(MarketingSeedData.Temperatures);
            modelBuilder.Entity<MarketingOwner>().HasData(MarketingSeedData.Owners);
            modelBuilder.Entity<MarketingLead>().HasData(MarketingSeedData.Leads);
            modelBuilder.Entity<MarketingLeadOwner>().HasData(MarketingSeedData.LeadOwners);
        }
    }
}
