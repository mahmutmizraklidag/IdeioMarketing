using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingOwner
    {
        // Pazarlama sorumlusunun benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Pazarlama sorumlusunun ekranda gösterilen adıdır.
        [MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        // Sorumlu için kullanılan renk kodudur.
        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        // Sorumluların ekrandaki sıralama değeridir.
        public int SortOrder { get; set; }

        // Sorumlunun atandığı potansiyel müşteri ilişkilerini tutar.
        public virtual ICollection<MarketingLeadOwner> LeadOwners { get; set; } = new List<MarketingLeadOwner>();
    }
}
