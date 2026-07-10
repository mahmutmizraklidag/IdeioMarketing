namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingLeadOwner
    {
        // İlişkinin bağlı olduğu potansiyel müşteri kaydının numarasıdır.
        public int MarketingLeadId { get; set; }

        // İlişkinin bağlı olduğu pazarlama sorumlusu kaydının numarasıdır.
        public int MarketingOwnerId { get; set; }

        // Aynı potansiyel müşterideki sorumlu sıralamasını belirtir.
        public int SortOrder { get; set; }

        // İlişkinin bağlı olduğu potansiyel müşteri bilgisidir.
        public virtual MarketingLead MarketingLead { get; set; } = null!;

        // İlişkinin bağlı olduğu pazarlama sorumlusu bilgisidir.
        public virtual MarketingOwner MarketingOwner { get; set; } = null!;
    }
}
