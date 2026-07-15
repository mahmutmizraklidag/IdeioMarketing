using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.MarketingFeature.Entities
{
    public class MarketingLead
    {
        // Potansiyel müşteri kaydının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Dış sistemlerden gelen veya ekranda kullanılan takip numarasıdır.
        [MaxLength(40)]
        public string ExternalId { get; set; } = string.Empty;

        // Potansiyel müşterinin firma adıdır.
        [MaxLength(250)]
        public string Company { get; set; } = string.Empty;

        // Firmadaki ilgili kişi adıdır.
        [MaxLength(250)]
        public string? Contact { get; set; }

        // Potansiyel müşterinin e-posta adresidir.
        [MaxLength(250)]
        public string? Email { get; set; }

        // Potansiyel müşterinin geldiği kaynak kaydının numarasıdır.
        public int SourceId { get; set; }

        // Potansiyel müşterinin mevcut durum kaydının numarasıdır.
        public int StatusId { get; set; }

        // Potansiyel müşterinin sıcaklık derecesi kaydının numarasıdır.
        public int TemperatureId { get; set; }

        // Potansiyel müşterinin süreç aşaması kaydının numarasıdır.
        public int StageId { get; set; }
        
        // Potansiyel müşteri için tahmini fırsat değeridir.
        public decimal Value { get; set; }

        // Potansiyel müşteriyle ilişkili takip veya kayıt tarihidir.
        public DateTime Date { get; set; }

        // Potansiyel müşteriyle ilgili serbest not alanıdır.
        public string? Note { get; set; }

        // Liste veya kanban görünümündeki sıralama değeridir.
        public int SortOrder { get; set; }

        // Kaydın pipeline (kanban) ekranında gösterilip gösterilmeyeceğini belirtir.
        // Yeni kayıtlar varsayılan olarak pipeline'a dahil edilir.
        public bool IsInPipeline { get; set; } = true;

        // Kaydın oluşturulma tarihidir.
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Kaydın son güncellenme tarihidir.
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Potansiyel müşterinin geldiği kaynak bilgisidir.
        public virtual MarketingSource Source { get; set; } = null!;

        // Potansiyel müşterinin mevcut durum bilgisidir.
        public virtual MarketingLeadStatus Status { get; set; } = null!;

        // Potansiyel müşterinin sıcaklık derecesi bilgisidir.
        public virtual MarketingLeadTemperature Temperature { get; set; } = null!;

        // Potansiyel müşterinin süreç aşaması bilgisidir.
        public virtual MarketingStage Stage { get; set; } = null!;

        // Potansiyel müşteriye atanmış sorumluları tutar.
        public virtual ICollection<MarketingLeadOwner> LeadOwners { get; set; } = new List<MarketingLeadOwner>();
    }
}
