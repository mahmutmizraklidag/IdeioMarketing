using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingLeadTemperature
    {
        // Sıcaklık derecesi kaydının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Sıcaklık derecesini kod tarafında ayırt etmek için kullanılan anahtardır.
        [MaxLength(50)]
        public string Key { get; set; } = string.Empty;

        // Sıcaklık derecesinin ekranda gösterilen adıdır.
        [MaxLength(100)]
        public string Label { get; set; } = string.Empty;

        // Sıcaklık derecesi için kullanılan ana renk kodudur.
        [MaxLength(20)]
        public string Color { get; set; } = string.Empty;

        // Sıcaklık derecesi için kullanılan yumuşak arka plan renk kodudur.
        [MaxLength(40)]
        public string SoftColor { get; set; } = string.Empty;

        // Sıcaklık derecelerinin ekrandaki sıralama değeridir.
        public int SortOrder { get; set; }

        // Bu sıcaklık derecesindeki potansiyel müşteri kayıtlarını tutar.
        public virtual ICollection<MarketingLead> Leads { get; set; } = new List<MarketingLead>();
    }
}
