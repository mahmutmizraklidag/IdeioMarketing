using IdeioMarketing.Areas.Admin.Models;
using IdeioMarketing.Data;
using IdeioMarketing.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeioMarketing.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PaymentPlansController : Controller
    {
        private readonly DatabaseContext _context;

        public PaymentPlansController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await BuildPageVmAsync(new PaymentPlanFormVm());
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.PaymentPlans
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.NumberOfInstallments,
                    x.DiscountRate
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Ödeme planı bulunamadı."
                });
            }

            return Json(new
            {
                success = true,
                item
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAjax(PaymentPlanFormVm model)
        {
            model.Name = (model.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError(nameof(model.Name), "Ödeme planı adı boş bırakılamaz.");

            if (model.Name.Length > 150)
                ModelState.AddModelError(nameof(model.Name), "Ödeme planı adı en fazla 150 karakter olabilir.");

            if (model.NumberOfInstallments < 1 || model.NumberOfInstallments > 120)
                ModelState.AddModelError(nameof(model.NumberOfInstallments), "Taksit sayısı 1 ile 120 arasında olmalıdır.");

            if (model.DiscountRate < 0 || model.DiscountRate > 100)
                ModelState.AddModelError(nameof(model.DiscountRate), "İndirim oranı 0 ile 100 arasında olmalıdır.");

            var exists = await _context.PaymentPlans
                .AnyAsync(x => x.Name == model.Name && x.Id != model.Id);

            if (exists)
                ModelState.AddModelError(nameof(model.Name), "Bu ödeme planı adı zaten kayıtlı.");

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Form doğrulama hatası oluştu.",
                    errors = GetModelErrors()
                });
            }

            var isNew = model.Id == 0;
            PaymentPlan entity;

            if (isNew)
            {
                entity = new PaymentPlan
                {
                    Name = model.Name,
                    NumberOfInstallments = model.NumberOfInstallments,
                    DiscountRate = model.DiscountRate
                };

                _context.PaymentPlans.Add(entity);
            }
            else
            {
                entity = await _context.PaymentPlans.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (entity == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Güncellenecek ödeme planı bulunamadı."
                    });
                }

                entity.Name = model.Name;
                entity.NumberOfInstallments = model.NumberOfInstallments;
                entity.DiscountRate = model.DiscountRate;
            }

            await _context.SaveChangesAsync();

            var item = await _context.PaymentPlans
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.NumberOfInstallments,
                    x.DiscountRate
                })
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return Json(new
            {
                success = true,
                message = isNew ? "Ödeme planı başarıyla eklendi." : "Ödeme planı başarıyla güncellendi.",
                item
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var entity = await _context.PaymentPlans.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Silinecek ödeme planı bulunamadı."
                });
            }

            _context.PaymentPlans.Remove(entity);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Ödeme planı başarıyla silindi.",
                id
            });
        }

        private async Task<PaymentPlanPageVm> BuildPageVmAsync(PaymentPlanFormVm? form = null)
        {
            var items = await _context.PaymentPlans
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new PaymentPlanListItemVm
                {
                    Id = x.Id,
                    Name = x.Name,
                    NumberOfInstallments = x.NumberOfInstallments,
                    DiscountRate = x.DiscountRate
                })
                .ToListAsync();

            return new PaymentPlanPageVm
            {
                Form = form ?? new PaymentPlanFormVm(),
                Items = items
            };
        }

        private List<string> GetModelErrors()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Geçersiz alan." : e.ErrorMessage)
                .Distinct()
                .ToList();
        }
    }
}