using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingSource
    {
        // Kaynak kaydının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Potansiyel müşterinin geldiği kaynak adıdır.
        [MaxLength(120)]
        public string Name { get; set; } = string.Empty;

        // Kaynakların ekrandaki sıralama değeridir.
        public int SortOrder { get; set; }

        // Bu kaynaktan gelen potansiyel müşteri kayıtlarını tutar.
        public virtual ICollection<MarketingLead> Leads { get; set; } = new List<MarketingLead>();
    }
}
