using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.Entities
{
    public class OfferRecord
    {
        // Teklif kaydının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Müşteriye veya kullanıcıya gösterilen teklif numarasıdır.
        [MaxLength(50)]
        public string OfferNo { get; set; } = string.Empty;

        // Teklif verilen müşteri veya firma adıdır.
        [MaxLength(250)]
        public string CustomerName { get; set; } = string.Empty;

        // Müşteri e-posta adresidir.
        [MaxLength(250)]
        public string? Email { get; set; }

        // Müşteri telefon numarasıdır.
        [MaxLength(50)]
        public string? Phone { get; set; }

        // Müşterinin bağlı olduğu vergi dairesidir.
        [MaxLength(150)]
        public string? TaxOffice { get; set; }

        // Müşterinin vergi numarasıdır.
        [MaxLength(50)]
        public string? TaxNumber { get; set; }

        // Teklif bildirimlerinde kullanılacak adres bilgisidir.
        public string? NotificationAddress { get; set; }

        // Teklifte seçilen ödeme planı kaydının numarasıdır.
        public int? PaymentPlanId { get; set; }

        // Teklif oluşturulduğu anda seçilen ödeme planı adıdır.
        [MaxLength(150)]
        public string? PaymentPlanName { get; set; }

        // Teklif oluşturulduğu anda ödeme planındaki taksit sayısıdır.
        public int PaymentPlanInstallmentCount { get; set; }

        // Teklif kapsamındaki sözleşme süresini ay olarak tutar.
        public int ContractDurationMonths { get; set; } = 1;

        // Teklife uygulanan indirim oranıdır.
        public decimal DiscountRate { get; set; }

        // Teklife uygulanan toplam indirim tutarıdır.
        public decimal DiscountAmount { get; set; }

        // İndirim uygulanmadan önceki toplam teklif tutarıdır.
        public decimal GrossTotal { get; set; }

        // İndirim sonrası toplam teklif tutarıdır.
        public decimal NetTotal { get; set; }

        // Kaydın teklif, sözleşme veya benzeri belge türünü belirtir.
        [MaxLength(50)]
        public string DocumentType { get; set; } = "proposal";

        // Teklif kaydının mevcut durumunu belirtir.
        [MaxLength(50)]
        public string Status { get; set; } = "Saved";

        // Teklif kaydının oluşturulma tarihidir.
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Teklifte seçilen paket ve alt paket satırlarını tutar.
        public virtual ICollection<OfferRecordItem> Items { get; set; }

        // Teklif için hesaplanan taksit satırlarını tutar.
        public virtual ICollection<OfferRecordInstallment> Installments { get; set; }

        public OfferRecord()
        {
            Items = new List<OfferRecordItem>();
            Installments = new List<OfferRecordInstallment>();
        }
    }
}
