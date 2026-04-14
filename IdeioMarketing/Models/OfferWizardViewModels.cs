namespace IdeioMarketing.Models
{
    public class OfferWizardViewModel
    {
        public List<OfferCategoryVm> Categories { get; set; } = new();
        public List<OfferPaymentPlanVm> PaymentPlans { get; set; } = new();
    }

    public class OfferCategoryVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public bool IsPackageMultiSelected { get; set; }
        public bool IsSubPackageMultiSelected { get; set; }

        public List<OfferPackageVm> Packages { get; set; } = new();
        public List<OfferSubPackageVm> SubPackages { get; set; } = new();
    }

    public class OfferPackageVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int? Piece { get; set; }
        public bool IsPiece { get; set; }

        public bool IsOneTime { get; set; }

        public List<string> Features { get; set; } = new();
    }

    public class OfferSubPackageVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int? Piece { get; set; }
        public bool IsPiece { get; set; }

        public bool IsOneTime { get; set; }

        public List<string> Features { get; set; } = new();
    }

    public class OfferPaymentPlanVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int NumberOfInstallments { get; set; }

        public int DiscountRate { get; set; }
    }
}