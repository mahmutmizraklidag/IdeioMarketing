namespace IdeioMarketing.Entities
{
    public class OfferRecordInstallment
    {
        public int Id { get; set; }

        public int OfferRecordId { get; set; }
        public virtual OfferRecord? OfferRecord { get; set; }

        public int MonthNo { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
    }
}