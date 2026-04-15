using IdeioMarketing.Areas.Admin.Models;
using IdeioMarketing.Data;
using IdeioMarketing.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeioMarketing.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize]
    public class OffersController : Controller
    {
        private readonly DatabaseContext _context;

        public OffersController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? q, string? documentType, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.OfferRecords
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.Installments)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                query = query.Where(x =>
                    x.OfferNo.Contains(q) ||
                    x.CustomerName.Contains(q) ||
                    (x.Email != null && x.Email.Contains(q)) ||
                    (x.Phone != null && x.Phone.Contains(q)));
            }

            if (!string.IsNullOrWhiteSpace(documentType))
            {
                query = query.Where(x => x.DocumentType == documentType);
            }

            if (fromDate.HasValue)
            {
                var start = fromDate.Value.Date;
                query = query.Where(x => x.CreatedAt >= start);
            }

            if (toDate.HasValue)
            {
                var end = toDate.Value.Date.AddDays(1);
                query = query.Where(x => x.CreatedAt < end);
            }

            var offers = await query
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new OfferAdminListItemVm
                {
                    Id = x.Id,
                    OfferNo = x.OfferNo,
                    CustomerName = x.CustomerName,
                    Email = x.Email,
                    Phone = x.Phone,
                    DocumentType = x.DocumentType,
                    DocumentTypeText = GetDocumentTypeText(x.DocumentType),
                    Status = x.Status,
                    GrossTotal = x.GrossTotal,
                    NetTotal = x.NetTotal,
                    ItemCount = x.Items.Count,
                    InstallmentCount = x.Installments.Count,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();

            var vm = new OfferAdminIndexVm
            {
                Q = q,
                DocumentType = documentType,
                FromDate = fromDate,
                ToDate = toDate,
                TotalCount = offers.Count,
                TotalGrossAmount = offers.Sum(x => x.GrossTotal),
                TotalNetAmount = offers.Sum(x => x.NetTotal),
                Offers = offers
            };

            return View(vm);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var offer = await _context.OfferRecords
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.Installments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (offer == null)
            {
                return NotFound();
            }

            var vm = new OfferAdminDetailVm
            {
                Offer = offer,
                DocumentTypeText = GetDocumentTypeText(offer.DocumentType)
            };

            return View(vm);
        }

        public async Task<IActionResult> Document(int id, bool autoDownload = false)
        {
            var offer = await _context.OfferRecords
                .AsNoTracking()
                .Include(x => x.Items)
                .Include(x => x.Installments)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (offer == null)
            {
                return NotFound();
            }

            var vm = new OfferAdminDocumentVm
            {
                Offer = offer,
                DocumentTypeText = GetDocumentTypeText(offer.DocumentType),
                AutoDownload = autoDownload
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var offer = await _context.OfferRecords.FirstOrDefaultAsync(x => x.Id == id);

            if (offer == null)
            {
                TempData["OfferAdminMessage"] = "Kayıt bulunamadı.";
                TempData["OfferAdminMessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            _context.OfferRecords.Remove(offer);
            await _context.SaveChangesAsync();

            TempData["OfferAdminMessage"] = $"'{offer.OfferNo}' numaralı teklif silindi.";
            TempData["OfferAdminMessageType"] = "success";

            return RedirectToAction(nameof(Index));
        }

        private static string GetDocumentTypeText(string? documentType)
        {
            return documentType switch
            {
                "contract" => "Hizmet Sözleşmesi",
                "proposal" => "Ön Protokol",
                _ => "Belirtilmemiş"
            };
        }
    }
}