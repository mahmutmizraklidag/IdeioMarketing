using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingStage
    {
        // Aşama kaydının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Aşamayı kod tarafında ayırt etmek için kullanılan anahtardır.
        [MaxLength(50)]
        public string Key { get; set; } = string.Empty;

        // Aşamanın ekranda gösterilen adıdır.
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        // Aşama için kullanılan renk kodudur.
        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        // Aşamaların ekrandaki sıralama değeridir.
        public int SortOrder { get; set; }

        // Bu aşamada bulunan potansiyel müşteri kayıtlarını tutar.
        public virtual ICollection<MarketingLead> Leads { get; set; } = new List<MarketingLead>();
    }
}
