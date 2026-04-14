using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdeioMarketing.Areas.Admin.Models
{
    public class CategoryFormVm
    {
        public int Id { get; set; }

        [Display(Name = "Kategori Adı")]
        [Required(ErrorMessage = "Kategori adı boş bırakılamaz.")]
        [StringLength(150, ErrorMessage = "Kategori adı en fazla 150 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Paketlerde çoklu seçim")]
        public bool IsPackageMultiSelected { get; set; }

        [Display(Name = "Alt paketlerde çoklu seçim")]
        public bool IsSubPackageMultiSelected { get; set; }
    }

    public class CategoryListItemVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPackageMultiSelected { get; set; }
        public bool IsSubPackageMultiSelected { get; set; }
        public int PackageCount { get; set; }
        public int SubPackageCount { get; set; }
    }

    public class CategoryPageVm
    {
        public CategoryFormVm Form { get; set; } = new();
        public List<CategoryListItemVm> Items { get; set; } = new();
    }

    public class PackageFormVm
    {
        public int Id { get; set; }

        [Display(Name = "Kategori")]
        [Required(ErrorMessage = "Kategori seçiniz.")]
        public int? CategoryId { get; set; }

        [Display(Name = "Paket Adı")]
        [Required(ErrorMessage = "Paket adı boş bırakılamaz.")]
        [StringLength(150, ErrorMessage = "Paket adı en fazla 150 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Fiyat")]
        [Required(ErrorMessage = "Fiyat boş bırakılamaz.")]
        public string Price { get; set; } = string.Empty;

        [Display(Name = "Parça Kullanılsın mı?")]
        public bool IsPiece { get; set; }

        [Display(Name = "Adet / Parça")]
        public int? Piece { get; set; }

        [Display(Name = "Tek Seferlik Mi")]
        public bool IsOneTime { get; set; }

        public List<string> FeatureTitles { get; set; } = new() { "" };
    }

    public class PackageListItemVm
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public bool IsPiece { get; set; }
        public int? Piece { get; set; }
        public bool IsOneTime { get; set; }
        public List<string> FeatureTitles { get; set; } = new();
    }

    public class PackagePageVm
    {
        public PackageFormVm Form { get; set; } = new();
        public List<PackageListItemVm> Items { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> CategoryTabs { get; set; } = new();
    }

    public class SubPackageFormVm
    {
        public int Id { get; set; }

        [Display(Name = "Kategori")]
        [Required(ErrorMessage = "Kategori seçiniz.")]
        public int? CategoryId { get; set; }

        [Display(Name = "Alt Paket Adı")]
        [Required(ErrorMessage = "Alt paket adı boş bırakılamaz.")]
        [StringLength(150, ErrorMessage = "Alt paket adı en fazla 150 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Fiyat")]
        [Required(ErrorMessage = "Fiyat boş bırakılamaz.")]
        public string Price { get; set; } = string.Empty;

        [Display(Name = "Parça Kullanılsın mı?")]
        public bool IsPiece { get; set; }

        [Display(Name = "Adet / Parça")]
        public int? Piece { get; set; }

        [Display(Name = "Tek Seferlik Mi")]
        public bool IsOneTime { get; set; }

        public List<string> FeatureTitles { get; set; } = new() { "" };
    }

    public class SubPackageListItemVm
    {
        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Price { get; set; } = string.Empty;
        public bool IsPiece { get; set; }
        public int? Piece { get; set; }
        public bool IsOneTime { get; set; }
        public List<string> FeatureTitles { get; set; } = new();
    }

    public class SubPackagePageVm
    {
        public SubPackageFormVm Form { get; set; } = new();
        public List<SubPackageListItemVm> Items { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> CategoryTabs { get; set; } = new();
    }

    public class UserFormVm
    {
        public int Id { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        [Required(ErrorMessage = "Kullanıcı adı boş bırakılamaz.")]
        [StringLength(30, ErrorMessage = "Kullanıcı adı en fazla 30 karakter olabilir.")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Şifre")]
        [Required(ErrorMessage = "Şifre boş bırakılamaz.")]
        [StringLength(30, ErrorMessage = "Şifre en fazla 30 karakter olabilir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class UserListItemVm
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
    }

    public class UserPageVm
    {
        public UserFormVm Form { get; set; } = new();
        public List<UserListItemVm> Items { get; set; } = new();
    }

    public class PaymentPlanFormVm
    {
        public int Id { get; set; }

        [Display(Name = "Ödeme Planı Adı")]
        [Required(ErrorMessage = "Ödeme planı adı boş bırakılamaz.")]
        [StringLength(150, ErrorMessage = "Ödeme planı adı en fazla 150 karakter olabilir.")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Taksit Sayısı")]
        [Range(1, 120, ErrorMessage = "Taksit sayısı 1 ile 120 arasında olmalıdır.")]
        public int NumberOfInstallments { get; set; } = 1;

        [Display(Name = "İndirim Oranı")]
        [Range(0, 100, ErrorMessage = "İndirim oranı 0 ile 100 arasında olmalıdır.")]
        public int DiscountRate { get; set; }
    }

    public class PaymentPlanListItemVm
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NumberOfInstallments { get; set; }
        public int DiscountRate { get; set; }
    }

    public class PaymentPlanPageVm
    {
        public PaymentPlanFormVm Form { get; set; } = new();
        public List<PaymentPlanListItemVm> Items { get; set; } = new();
    }
}