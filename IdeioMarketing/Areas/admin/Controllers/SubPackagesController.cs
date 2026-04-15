using IdeioMarketing.Areas.Admin.Models;
using IdeioMarketing.Data;
using IdeioMarketing.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace IdeioMarketing.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize]
    public class SubPackagesController : Controller
    {
        private readonly DatabaseContext _context;

        public SubPackagesController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? editId = null)
        {
            SubPackageFormVm form = new();

            if (editId.HasValue)
            {
                var entity = await _context.SubPackages
                    .AsNoTracking()
                    .Include(x => x.PackageFeatures)
                    .FirstOrDefaultAsync(x => x.Id == editId.Value);

                if (entity != null)
                {
                    form.Id = entity.Id;
                    form.Name = entity.Name;
                    form.Price = entity.Price;
                    form.Piece = entity.Piece;
                    form.IsPiece = entity.IsPiece;
                    form.IsOneTime = entity.IsOneTime;
                    form.CategoryId = entity.CategoryId;
                    form.FeatureTitles = entity.PackageFeatures
                        .OrderBy(x => x.Id)
                        .Select(x => x.Title)
                        .ToList();

                    if (!form.FeatureTitles.Any())
                        form.FeatureTitles.Add("");
                }
            }

            var vm = await BuildPageVmAsync(form);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save([Bind(Prefix = "Form")] SubPackageFormVm model)
        {
            NormalizeFeatureTitles(model);

            if (!model.CategoryId.HasValue)
                ModelState.AddModelError(nameof(model.CategoryId), "Kategori seçiniz.");

           

            if (model.IsPiece)
            {
                if (!model.Piece.HasValue || model.Piece.Value <= 0)
                    ModelState.AddModelError(nameof(model.Piece), "Parça kullanımı açıksa geçerli bir adet/parça giriniz.");
            }
            else
            {
                model.Piece = null;
            }

            if (!ModelState.IsValid)
            {
                var invalidVm = await BuildPageVmAsync(model);
                return View("Index", invalidVm);
            }

            SubPackage entity;
            var isNew = model.Id == 0;

            if (isNew)
            {
                entity = new SubPackage();
                _context.SubPackages.Add(entity);
            }
            else
            {
                entity = await _context.SubPackages.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (entity == null)
                    return RedirectToAction(nameof(Index));
            }

            entity.Name = model.Name.Trim();
            entity.Price = model.Price.Trim();
            entity.IsPiece = model.IsPiece;
            entity.Piece = model.IsPiece ? model.Piece : null;
            entity.IsOneTime = model.IsOneTime;
            entity.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();

            var oldFeatures = await _context.PackageFeatures
                .Where(x => x.SubPackageId == entity.Id)
                .ToListAsync();

            if (oldFeatures.Any())
                _context.PackageFeatures.RemoveRange(oldFeatures);

            var newFeatures = model.FeatureTitles
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new PackageFeatures
                {
                    Title = x.Trim(),
                    SubPackageId = entity.Id
                })
                .ToList();

            if (newFeatures.Any())
                _context.PackageFeatures.AddRange(newFeatures);

            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = isNew
                ? "Alt paket başarıyla eklendi."
                : "Alt paket başarıyla güncellendi.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.SubPackages
                .Include(x => x.PackageFeatures)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return RedirectToAction(nameof(Index));

            if (entity.PackageFeatures.Any())
                _context.PackageFeatures.RemoveRange(entity.PackageFeatures);

            _context.SubPackages.Remove(entity);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Alt paket başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<SubPackagePageVm> BuildPageVmAsync(SubPackageFormVm? form = null)
        {
            form ??= new SubPackageFormVm();

            if (form.FeatureTitles == null || !form.FeatureTitles.Any())
                form.FeatureTitles = new List<string> { "" };

            var entities = await _context.SubPackages
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.PackageFeatures)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var items = entities.Select(x => new SubPackageListItemVm
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                Name = x.Name,
                Price = x.Price,
                IsPiece = x.IsPiece,
                Piece = x.Piece,
                IsOneTime = x.IsOneTime,
                CategoryName = x.Category?.Name ?? "-",
                FeatureTitles = x.PackageFeatures
        .OrderBy(f => f.Id)
        .Select(f => f.Title)
        .ToList()
            }).ToList();

            var categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            return new SubPackagePageVm
            {
                Form = form,
                Items = items,
                Categories = categories,
                CategoryTabs = categories
            };
        }

        private static void NormalizeFeatureTitles(SubPackageFormVm model)
        {
            model.FeatureTitles = (model.FeatureTitles ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct()
                .ToList();
        }
    }
}