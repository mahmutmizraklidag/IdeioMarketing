using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingLeadStatus
    {
        // Durum kaydının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Durumu kod tarafında ayırt etmek için kullanılan anahtardır.
        [MaxLength(50)]
        public string Key { get; set; } = string.Empty;

        // Durumun ekranda gösterilen adıdır.
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        // Durumun tekrar eden müşteri sürecini ifade edip etmediğini belirtir.
        public bool IsRecurring { get; set; }

        // Durumların ekrandaki sıralama değeridir.
        public int SortOrder { get; set; }

        // Bu durumda bulunan potansiyel müşteri kayıtlarını tutar.
        public virtual ICollection<MarketingLead> Leads { get; set; } = new List<MarketingLead>();
    }
}
