using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingSource
    {
        public int Id { get; set; }

        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public virtual ICollection<MarketingLead> Leads { get; set; } = new List<MarketingLead>();
    }
}
