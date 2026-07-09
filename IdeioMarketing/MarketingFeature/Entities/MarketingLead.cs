using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingLead
    {
        public int Id { get; set; }

        [MaxLength(40)]
        public string ExternalId { get; set; } = string.Empty;

        [MaxLength(250)]
        public string Company { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Contact { get; set; }

        [MaxLength(250)]
        public string? Email { get; set; }

        public int SourceId { get; set; }
        public int StatusId { get; set; }
        public int TemperatureId { get; set; }
        public int StageId { get; set; }

        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public string? Note { get; set; }
        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public virtual MarketingSource Source { get; set; } = null!;
        public virtual MarketingLeadStatus Status { get; set; } = null!;
        public virtual MarketingLeadTemperature Temperature { get; set; } = null!;
        public virtual MarketingStage Stage { get; set; } = null!;
        public virtual ICollection<MarketingLeadOwner> LeadOwners { get; set; } = new List<MarketingLeadOwner>();
    }
}
