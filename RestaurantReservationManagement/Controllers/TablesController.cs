using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using RestaurantBusiness.Models;

namespace FUCourseManagement.Controllers
{
    public class TablesController : BaseController<Table, int>
    {
        private ITableRepository tableRepository;

        public TablesController(ITableRepository repository)
            : base(repository)
        {
            tableRepository = repository;
        }

        public override async Task<IActionResult> Index()
        {
            var tables = await _repository.GetAllAsync();
            if (User.IsInRole("User"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var status = new Dictionary<int, bool>();
                foreach (var table in tables)
                {
                    status[table.Id] = await tableRepository.CheckStatusAsync(table.Id, userId);
                }
                ViewData["Status"] = status;
            }
            ViewData["Title"] = _entityName;
            return View("~/Views/Shared/Generic/Index.cshtml", tables);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                await tableRepository.BookAsync(id, int.Parse(userId), DateTime.Now);
                TempData["SuccessMessage"] = "Đặt bàn thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Book(int id, DateTime dateTime)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                await tableRepository.BookAsync(id, int.Parse(userId), dateTime);
                TempData["SuccessMessage"] = "Đặt bàn thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
