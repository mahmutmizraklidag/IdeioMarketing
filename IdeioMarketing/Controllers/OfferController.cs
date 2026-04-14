using IdeioMarketing.Data;
using IdeioMarketing.Entities;
using IdeioMarketing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace IdeioMarketing.Controllers
{
    public class OfferController : Controller
    {
        private readonly DatabaseContext _context;

        public OfferController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Include(x => x.Packages)
                    .ThenInclude(x => x.PackageFeatures)
                .Include(x => x.SubPackages)
                    .ThenInclude(x => x.PackageFeatures)
                .OrderBy(x => x.Id)
                .ToListAsync();

            var paymentPlans = await _context.PaymentPlans
                .AsNoTracking()
                .OrderBy(x => x.NumberOfInstallments)
                .ThenBy(x => x.Id)
                .ToListAsync();

            var model = new OfferWizardViewModel
            {
                Categories = categories.Select(category => new OfferCategoryVm
                {
                    Id = category.Id,
                    Name = category.Name,
                    IsPackageMultiSelected = category.IsPackageMultiSelected,
                    IsSubPackageMultiSelected = category.IsSubPackageMultiSelected,

                    Packages = category.Packages?
                        .OrderBy(x => x.Id)
                        .Select(package => new OfferPackageVm
                        {
                            Id = package.Id,
                            Name = package.Name,
                            Price = ParsePrice(package.Price),
                            IsPiece = package.IsPiece,
                            Piece = package.Piece,
                            IsOneTime = package.IsOneTime,
                            Features = package.PackageFeatures?
                                .OrderBy(x => x.Id)
                                .Where(x => !string.IsNullOrWhiteSpace(x.Title))
                                .Select(x => x.Title)
                                .ToList() ?? new List<string>()
                        })
                        .ToList() ?? new List<OfferPackageVm>(),

                    SubPackages = category.SubPackages?
                        .OrderBy(x => x.Id)
                        .Select(subPackage => new OfferSubPackageVm
                        {
                            Id = subPackage.Id,
                            Name = subPackage.Name,
                            Price = ParsePrice(subPackage.Price),
                            IsPiece = subPackage.IsPiece,
                            Piece = subPackage.Piece,
                            IsOneTime = subPackage.IsOneTime,
                            Features = subPackage.PackageFeatures?
                                .OrderBy(x => x.Id)
                                .Where(x => !string.IsNullOrWhiteSpace(x.Title))
                                .Select(x => x.Title)
                                .ToList() ?? new List<string>()
                        })
                        .ToList() ?? new List<OfferSubPackageVm>()
                }).ToList(),

                PaymentPlans = paymentPlans.Select(plan => new OfferPaymentPlanVm
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    NumberOfInstallments = plan.NumberOfInstallments,
                    DiscountRate = plan.DiscountRate
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(SaveOfferFormRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CustomerName))
                {
                    return Json(new { success = false, message = "Müşteri / firma adı zorunludur." });
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var items = string.IsNullOrWhiteSpace(request.SelectedItemsJson)
                    ? new List<OfferSaveItemPayload>()
                    : JsonSerializer.Deserialize<List<OfferSaveItemPayload>>(request.SelectedItemsJson, jsonOptions) ?? new List<OfferSaveItemPayload>();

                var installments = string.IsNullOrWhiteSpace(request.InstallmentsJson)
                    ? new List<OfferSaveInstallmentPayload>()
                    : JsonSerializer.Deserialize<List<OfferSaveInstallmentPayload>>(request.InstallmentsJson, jsonOptions) ?? new List<OfferSaveInstallmentPayload>();

                if (!items.Any())
                {
                    return Json(new { success = false, message = "Kaydetmeden önce en az bir paket veya alt paket seçmelisiniz." });
                }

                var offer = new OfferRecord
                {
                    OfferNo = GenerateOfferNo(),
                    CustomerName = request.CustomerName?.Trim() ?? string.Empty,
                    Email = request.Email?.Trim(),
                    Phone = request.Phone?.Trim(),
                    TaxOffice = request.TaxOffice?.Trim(),
                    TaxNumber = request.TaxNumber?.Trim(),
                    NotificationAddress = request.NotificationAddress?.Trim(),
                    PaymentPlanId = request.PaymentPlanId,
                    PaymentPlanName = request.PaymentPlanName?.Trim(),
                    PaymentPlanInstallmentCount = request.PaymentPlanInstallmentCount,
                    DiscountRate = ParseDecimalFlexible(request.DiscountRate),
                    DiscountAmount = ParseDecimalFlexible(request.DiscountAmount),
                    GrossTotal = ParseDecimalFlexible(request.GrossTotal),
                    NetTotal = ParseDecimalFlexible(request.NetTotal),
                    DocumentType = string.IsNullOrWhiteSpace(request.DocumentType) ? "proposal" : request.DocumentType.Trim(),
                    Status = "Kaydedildi",
                    CreatedAt = DateTime.Now
                };

                foreach (var item in items)
                {
                    offer.Items.Add(new OfferRecordItem
                    {
                        ItemType = item.ItemType,
                        SourceItemId = item.Id,
                        CategoryId = item.CategoryId,
                        CategoryName = item.CategoryName,
                        Name = item.Name,
                        RawName = item.RawName,
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        TotalPrice = item.Amount,
                        IsOneTime = item.IsOneTime,
                        IsPiece = item.IsPiece
                    });
                }

                foreach (var installment in installments)
                {
                    offer.Installments.Add(new OfferRecordInstallment
                    {
                        MonthNo = installment.Month,
                        GrossAmount = installment.Gross,
                        NetAmount = installment.Net
                    });
                }

                _context.OfferRecords.Add(offer);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    id = offer.Id,
                    offerNo = offer.OfferNo,
                    message = $"Teklif başarıyla kaydedildi. Kayıt No: {offer.OfferNo}"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Kayıt sırasında bir hata oluştu: " + ex.Message
                });
            }
        }

        private static decimal ParsePrice(string? rawPrice)
        {
            if (string.IsNullOrWhiteSpace(rawPrice))
                return 0m;

            rawPrice = rawPrice.Trim();

            if (decimal.TryParse(rawPrice, NumberStyles.Any, new CultureInfo("tr-TR"), out var trValue))
                return trValue;

            if (decimal.TryParse(rawPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out var invariantValue))
                return invariantValue;

            var cleaned = rawPrice
                .Replace("₺", "")
                .Replace("TL", "", StringComparison.OrdinalIgnoreCase)
                .Replace("TRY", "", StringComparison.OrdinalIgnoreCase)
                .Trim();

            if (cleaned.Contains(',') && cleaned.Contains('.'))
            {
                cleaned = cleaned.Replace(".", "").Replace(",", ".");
            }
            else if (cleaned.Contains(','))
            {
                cleaned = cleaned.Replace(",", ".");
            }

            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var cleanedValue))
                return cleanedValue;

            return 0m;
        }

        private static decimal ParseDecimalFlexible(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0m;

            value = value.Trim();

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var invariant))
                return invariant;

            if (decimal.TryParse(value, NumberStyles.Any, new CultureInfo("tr-TR"), out var tr))
                return tr;

            return 0m;
        }

        private static string GenerateOfferNo()
        {
            return $"TKF-{DateTime.Now:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";
        }
    }
}