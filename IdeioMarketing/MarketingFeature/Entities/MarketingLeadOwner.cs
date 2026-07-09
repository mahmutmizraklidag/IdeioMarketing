namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingLeadOwner
    {
        public int MarketingLeadId { get; set; }
        public int MarketingOwnerId { get; set; }
        public int SortOrder { get; set; }

        public virtual MarketingLead MarketingLead { get; set; } = null!;
        public virtual MarketingOwner MarketingOwner { get; set; } = null!;
    }
}
