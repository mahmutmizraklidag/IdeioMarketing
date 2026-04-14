using System.ComponentModel.DataAnnotations;

namespace IdeioMarketing.Entities
{
    public class OfferRecord
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string OfferNo { get; set; } = string.Empty;

        [MaxLength(250)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [MaxLength(150)]
        public string? TaxOffice { get; set; }

        [MaxLength(50)]
        public string? TaxNumber { get; set; }

        public string? NotificationAddress { get; set; }

        public int? PaymentPlanId { get; set; }

        [MaxLength(150)]
        public string? PaymentPlanName { get; set; }

        public int PaymentPlanInstallmentCount { get; set; }

        public decimal DiscountRate { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal GrossTotal { get; set; }
        public decimal NetTotal { get; set; }

        [MaxLength(50)]
        public string DocumentType { get; set; } = "proposal";

        [MaxLength(50)]
        public string Status { get; set; } = "Saved";

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<OfferRecordItem> Items { get; set; }
        public virtual ICollection<OfferRecordInstallment> Installments { get; set; }

        public OfferRecord()
        {
            Items = new List<OfferRecordItem>();
            Installments = new List<OfferRecordInstallment>();
        }
    }
}