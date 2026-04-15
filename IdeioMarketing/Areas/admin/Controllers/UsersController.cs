using IdeioMarketing.Areas.Admin.Models;
using IdeioMarketing.Data;
using IdeioMarketing.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdeioMarketing.Areas.Admin.Controllers
{
    [Area("Admin"),Authorize]
    public class UsersController : Controller
    {
        private readonly DatabaseContext _context;

        public UsersController(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? editId = null)
        {
            UserFormVm form = new();

            if (editId.HasValue)
            {
                var entity = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == editId.Value);

                if (entity != null)
                {
                    form.Id = entity.Id;
                    form.Username = entity.Username;
                    form.Password = entity.password;
                }
            }

            var vm = await BuildPageVmAsync(form);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(UserFormVm model)
        {
            if (!ModelState.IsValid)
            {
                var invalidVm = await BuildPageVmAsync(model);
                return View("Index", invalidVm);
            }

            model.Username = model.Username.Trim();
            model.Password = model.Password.Trim();

            var exists = await _context.Users
                .AnyAsync(x => x.Username == model.Username && x.Id != model.Id);

            if (exists)
            {
                ModelState.AddModelError(nameof(model.Username), "Bu kullanıcı adı zaten kullanılıyor.");
                var invalidVm = await BuildPageVmAsync(model);
                return View("Index", invalidVm);
            }

            IdeioMarketing.Entities.User entity;

            if (model.Id == 0)
            {
                entity = new IdeioMarketing.Entities.User();
                _context.Users.Add(entity);
                TempData["AdminSuccess"] = "Kullanıcı başarıyla eklendi.";
            }
            else
            {
                entity = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);

                if (entity == null)
                    return RedirectToAction(nameof(Index));

                TempData["AdminSuccess"] = "Kullanıcı başarıyla güncellendi.";
            }

            entity.Username = model.Username;
            entity.password = model.Password;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return RedirectToAction(nameof(Index));

            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();

            TempData["AdminSuccess"] = "Kullanıcı başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<UserPageVm> BuildPageVmAsync(UserFormVm? form = null)
        {
            var items = await _context.Users
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new UserListItemVm
                {
                    Id = x.Id,
                    Username = x.Username
                })
                .ToListAsync();

            return new UserPageVm
            {
                Form = form ?? new UserFormVm(),
                Items = items
            };
        }
    }
}