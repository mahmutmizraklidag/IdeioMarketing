using IdeioMarketing.Areas.Admin.Models;
using IdeioMarketing.Data;
using IdeioMarketing.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace IdeioMarketing.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly DatabaseContext _context;

        public CategoriesController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = await BuildPageVmAsync(new CategoryFormVm());
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Categories
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.IsPackageMultiSelected,
                    x.IsSubPackageMultiSelected,
                    PackageCount = x.Packages.Count,
                    SubPackageCount = x.SubPackages.Count
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Kategori bulunamadı."
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
        public async Task<IActionResult> SaveAjax(CategoryFormVm model)
        {
            model.Name = (model.Name ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(model.Name))
                ModelState.AddModelError(nameof(model.Name), "Kategori adı boş bırakılamaz.");

            if (model.Name.Length > 150)
                ModelState.AddModelError(nameof(model.Name), "Kategori adı en fazla 150 karakter olabilir.");

            var exists = await _context.Categories
                .AnyAsync(x => x.Name == model.Name && x.Id != model.Id);

            if (exists)
                ModelState.AddModelError(nameof(model.Name), "Bu kategori adı zaten kayıtlı.");

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
            Category entity;

            if (isNew)
            {
                entity = new Category
                {
                    Name = model.Name,
                    IsPackageMultiSelected = model.IsPackageMultiSelected,
                    IsSubPackageMultiSelected = model.IsSubPackageMultiSelected
                };

                _context.Categories.Add(entity);
            }
            else
            {
                entity = await _context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (entity == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Güncellenecek kategori bulunamadı."
                    });
                }

                entity.Name = model.Name;
                entity.IsPackageMultiSelected = model.IsPackageMultiSelected;
                entity.IsSubPackageMultiSelected = model.IsSubPackageMultiSelected;
            }

            await _context.SaveChangesAsync();

            var item = await _context.Categories
                .AsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.IsPackageMultiSelected,
                    x.IsSubPackageMultiSelected,
                    PackageCount = x.Packages.Count,
                    SubPackageCount = x.SubPackages.Count
                })
                .FirstOrDefaultAsync(x => x.Id == entity.Id);

            return Json(new
            {
                success = true,
                message = isNew ? "Kategori başarıyla eklendi." : "Kategori başarıyla güncellendi.",
                item
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var entity = await _context.Categories
                .Include(x => x.Packages)
                .Include(x => x.SubPackages)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Silinecek kategori bulunamadı."
                });
            }

            if (entity.Packages.Any() || entity.SubPackages.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "Bu kategori paket veya alt paket ile ilişkili olduğu için silinemez."
                });
            }

            _context.Categories.Remove(entity);
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Kategori başarıyla silindi.",
                id
            });
        }

        private async Task<CategoryPageVm> BuildPageVmAsync(CategoryFormVm? form = null)
        {
            var items = await _context.Categories
                .AsNoTracking()
                .Select(x => new CategoryListItemVm
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsPackageMultiSelected = x.IsPackageMultiSelected,
                    IsSubPackageMultiSelected = x.IsSubPackageMultiSelected,
                    PackageCount = x.Packages.Count,
                    SubPackageCount = x.SubPackages.Count
                })
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return new CategoryPageVm
            {
                Form = form ?? new CategoryFormVm(),
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