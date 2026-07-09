using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingOwner
    {
        public int Id { get; set; }

        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        public int SortOrder { get; set; }

        public virtual ICollection<MarketingLeadOwner> LeadOwners { get; set; } = new List<MarketingLeadOwner>();
    }
}
