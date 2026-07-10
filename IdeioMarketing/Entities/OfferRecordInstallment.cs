namespace IdeioMarketing.Entities
{
    public class OfferRecordInstallment
    {
        // Taksit satırının benzersiz kayıt numarasıdır.
        public int Id { get; set; }

        // Taksidin bağlı olduğu teklif kaydının numarasıdır.
        public int OfferRecordId { get; set; }

        // Taksidin bağlı olduğu teklif kaydıdır.
        public virtual OfferRecord? OfferRecord { get; set; }

        // Taksidin kaçıncı aya ait olduğunu belirtir.
        public int MonthNo { get; set; }

        // İndirim uygulanmadan önceki taksit tutarıdır.
        public decimal GrossAmount { get; set; }

        // İndirim sonrası taksit tutarıdır.
        public decimal NetAmount { get; set; }
    }
}
