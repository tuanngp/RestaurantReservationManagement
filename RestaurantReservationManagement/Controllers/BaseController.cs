using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace FUCourseManagement.Controllers
{
    [Authorize]
    public abstract class BaseController<TEntity, TId> : Controller
        where TEntity : class
    {
        protected readonly IBaseRepository<TEntity, TId> _repository;
        protected readonly string _entityName;

        protected BaseController(IBaseRepository<TEntity, TId> repository)
        {
            _repository = repository;
            _entityName = typeof(TEntity).Name;
        }

        // GET: Entity
        public virtual async Task<IActionResult> Index()
        {
            IEnumerable<TEntity>? entities = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                {
                    entities = await _repository.GetAllAsync();
                }
                else
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    entities = await _repository.GetAllAsync();

                    if (HasUserIdProperty(Activator.CreateInstance<TEntity>()))
                    {
                        entities = entities.Where(e =>
                            (int?)e.GetType().GetProperty("UserId")?.GetValue(e) == userId
                        );
                    }
                }
            }

            ViewData["Title"] = _entityName;
            return View("~/Views/Shared/Generic/Index.cshtml", entities);
        }

        protected virtual bool HasUserIdProperty(TEntity entity)
        {
            return entity.GetType().GetProperties().Any(p => p.Name == "UserId");
        }

        // GET: Entity/Details/5
        public virtual async Task<IActionResult> Details(TId id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            ViewData["Title"] = _entityName;
            return View("~/Views/Shared/Generic/Details.cshtml", entity);
        }

        // GET: Entity/Create
        [Authorize(Roles = "Admin")]
        public virtual IActionResult Create()
        {
            ViewData["Title"] = _entityName;
            var entity = Activator.CreateInstance<TEntity>();
            return View("~/Views/Shared/Generic/Create.cshtml", entity);
        }

        // POST: Entity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public virtual async Task<IActionResult> Create(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] =
                    "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin nhập.";
                TempData["IsCreateRetry"] = true;
                ViewData["Title"] = _entityName;
                return View("~/Views/Shared/Generic/Create.cshtml", entity);
            }

            try
            {
                await _repository.AddAsync(entity);
                TempData["SuccessMessage"] = $"{_entityName} đã được tạo thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi tạo {_entityName}: {ex.Message}";
                TempData["IsCreateRetry"] = true;
                ViewData["Title"] = _entityName;
                return View("~/Views/Shared/Generic/Create.cshtml", entity);
            }
        }

        // GET: Entity/Edit/5
        [Authorize(Roles = "Admin")]
        public virtual async Task<IActionResult> Edit(TId id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            ViewData["Title"] = _entityName;
            return View("~/Views/Shared/Generic/Edit.cshtml", entity);
        }

        // POST: Entity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public virtual async Task<IActionResult> Edit(TId id, TEntity entity)
        {
            var entityId = entity
                .GetType()
                .GetProperties()
                .FirstOrDefault(p => p.Name == "Id" || p.Name.EndsWith("Id"))
                ?.GetValue(entity);
            if (!entityId?.ToString().Equals(id?.ToString()) ?? true)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bản ghi với ID tương ứng.";
                return View("~/Views/Shared/Generic/Edit.cshtml", entity);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _repository.UpdateAsync(entity);
                    TempData["SuccessMessage"] = "Cập nhật bản ghi thành công.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    if (!await EntityExists(id))
                    {
                        TempData["ErrorMessage"] = "Không tìm thấy bản ghi để cập nhật.";
                        return View("~/Views/Shared/Generic/Edit.cshtml", entity);
                    }
                    TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi cập nhật bản ghi: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] =
                    "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin nhập.";
            }

            ViewData["Title"] = _entityName;
            return View("~/Views/Shared/Generic/Edit.cshtml", entity);
        }

        // GET: Entity/Delete/5
        [Authorize(Roles = "Admin")]
        public virtual async Task<IActionResult> Delete(TId id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            ViewData["Title"] = _entityName;
            return View("~/Views/Shared/Generic/Delete.cshtml", entity);
        }

        // POST: Entity/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(TId id)
        {
            try
            {
                var result = await _repository.DeleteAsync(id);
                if (!result)
                {
                    TempData["ErrorMessage"] = $"Không tìm thấy {_entityName} với ID: {id} để xóa.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = $"{_entityName} đã được xóa thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi xóa {_entityName}: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        protected virtual async Task<bool> EntityExists(TId id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity != null;
        }

        // GET: Entity/Search
        public virtual async Task<IActionResult> Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return RedirectToAction(nameof(Index));
            }

            IEnumerable<TEntity>? entities = null;

            if (User.Identity?.IsAuthenticated == true)
            {
                entities = await _repository.GetAllAsync();

                if (
                    !User.IsInRole("Admin")
                    && HasUserIdProperty(Activator.CreateInstance<TEntity>())
                )
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    entities = entities.Where(e =>
                        (int?)e.GetType().GetProperty("UserId")?.GetValue(e) == userId
                    );
                }

                var searchableProps = typeof(TEntity)
                    .GetProperties()
                    .Where(p =>
                        !p.PropertyType.IsGenericType
                        && !p.Name.Contains("Collection")
                        && !p.Name.EndsWith("Navigation")
                        && !p.Name.Contains("Password")
                        && (
                            p.PropertyType == typeof(string)
                            || p.PropertyType == typeof(int)
                            || p.PropertyType == typeof(decimal)
                            || p.PropertyType == typeof(DateTime)
                        )
                    );

                searchText = searchText.ToLower();
                entities = entities.Where(entity =>
                    searchableProps.Any(prop =>
                    {
                        var value = prop.GetValue(entity);
                        if (value == null)
                            return false;

                        return value.ToString()?.ToLower()?.Contains(searchText) ?? false;
                    })
                );
            }

            ViewData["Title"] = _entityName;
            ViewData["SearchText"] = searchText;
            return View("~/Views/Shared/Generic/Index.cshtml", entities);
        }
    }
}
