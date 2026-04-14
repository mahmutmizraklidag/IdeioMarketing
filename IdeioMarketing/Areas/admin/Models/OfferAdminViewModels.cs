using IdeioMarketing.Entities;

namespace IdeioMarketing.Areas.Admin.Models
{
    public class OfferAdminIndexVm
    {
        public string? Q { get; set; }
        public string? DocumentType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int TotalCount { get; set; }
        public decimal TotalNetAmount { get; set; }
        public decimal TotalGrossAmount { get; set; }

        public List<OfferAdminListItemVm> Offers { get; set; } = new();
    }

    public class OfferAdminListItemVm
    {
        public int Id { get; set; }
        public string OfferNo { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentTypeText { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal GrossTotal { get; set; }
        public decimal NetTotal { get; set; }
        public int ItemCount { get; set; }
        public int InstallmentCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class OfferAdminDetailVm
    {
        public OfferRecord Offer { get; set; } = new();
        public string DocumentTypeText { get; set; } = string.Empty;
    }
    public class OfferAdminDocumentVm
    {
        public OfferRecord Offer { get; set; } = new();
        public string DocumentTypeText { get; set; } = string.Empty;
        public bool AutoDownload { get; set; }
    }
}