namespace IdeioMarketing.Models
{
    public class SaveOfferFormRequest
    {
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public string? NotificationAddress { get; set; }

        public int? PaymentPlanId { get; set; }
        public string? PaymentPlanName { get; set; }
        public int PaymentPlanInstallmentCount { get; set; }

        public string? DiscountRate { get; set; }
        public string? DiscountAmount { get; set; }
        public string? GrossTotal { get; set; }
        public string? NetTotal { get; set; }

        public string? DocumentType { get; set; }

        public string? SelectedItemsJson { get; set; }
        public string? InstallmentsJson { get; set; }
    }

    public class OfferSaveItemPayload
    {
        public string ItemType { get; set; } = string.Empty;
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string RawName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public bool IsOneTime { get; set; }
        public bool IsPiece { get; set; }
    }

    public class OfferSaveInstallmentPayload
    {
        public int Month { get; set; }
        public decimal Gross { get; set; }
        public decimal Net { get; set; }
    }
}