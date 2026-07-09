using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingStage
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Key { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public virtual ICollection<MarketingLead> Leads { get; set; } = new List<MarketingLead>();
    }
}
