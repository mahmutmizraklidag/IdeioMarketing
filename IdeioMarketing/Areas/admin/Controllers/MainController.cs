using IdeioMarketing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeioMarketing.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize]
    public class MainController : Controller
    {
        private readonly DatabaseContext _context;

        public MainController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardVm
            {
                CategoryCount = await _context.Categories.CountAsync(),
                PackageCount = await _context.Packages.CountAsync(),
                SubPackageCount = await _context.SubPackages.CountAsync(),
                UserCount = await _context.Users.CountAsync(),

                LastCategories = await _context.Categories
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .Select(x => new DashboardMiniListItemVm
                    {
                        Id = x.Id,
                        Title = x.Name,
                        Description = "Kategori kaydı"
                    })
                    .ToListAsync(),

                LastPackages = await _context.Packages
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .Select(x => new DashboardMiniListItemVm
                    {
                        Id = x.Id,
                        Title = x.Name,
                        Description = (x.Category != null ? x.Category.Name : "-") + " / " + x.Price
                    })
                    .ToListAsync(),

                LastSubPackages = await _context.SubPackages
                    .AsNoTracking()
                    .Include(x => x.Category)
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .Select(x => new DashboardMiniListItemVm
                    {
                        Id = x.Id,
                        Title = x.Name,
                        Description = (x.Category != null ? x.Category.Name : "-") + " / " + x.Price
                    })
                    .ToListAsync(),

                LastUsers = await _context.Users
                    .AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Take(5)
                    .Select(x => new DashboardMiniListItemVm
                    {
                        Id = x.Id,
                        Title = x.Username,
                        Description = "Yönetici kullanıcı"
                    })
                    .ToListAsync()
            };

            return View(vm);
        }
    }

    public class DashboardVm
    {
        public int CategoryCount { get; set; }
        public int PackageCount { get; set; }
        public int SubPackageCount { get; set; }
        public int UserCount { get; set; }

        public List<DashboardMiniListItemVm> LastCategories { get; set; } = new();
        public List<DashboardMiniListItemVm> LastPackages { get; set; } = new();
        public List<DashboardMiniListItemVm> LastSubPackages { get; set; } = new();
        public List<DashboardMiniListItemVm> LastUsers { get; set; } = new();
    }

    public class DashboardMiniListItemVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}