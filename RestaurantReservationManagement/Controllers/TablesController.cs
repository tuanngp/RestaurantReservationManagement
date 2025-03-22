﻿﻿﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using RestaurantBusiness.Models;
using RestaurantReservationManagement.Models;

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
            ViewData["Title"] = "Table";
            return View("~/Views/Shared/Generic/Index.cshtml", tables);
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            var table = await _repository.GetByIdAsync(id);
            if (table == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bàn.";
                return RedirectToAction(nameof(Index));
            }

            var model = new BookTableViewModel
            {
                TableId = table.Id,
                TableNumber = table.TableNumber,
                Seats = table.Seats,
                ReservationDateTime = DateTime.Now.AddHours(1) // Mặc định là 1 giờ sau
            };

            return View(model);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CancelBooking(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                await tableRepository.CancelBookingAsync(id, int.Parse(userId));
                TempData["SuccessMessage"] = "Hủy đặt bàn thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> Book(BookTableViewModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            if (!ModelState.IsValid)
            {
                var table = await _repository.GetByIdAsync(model.TableId);
                if (table != null)
                {
                    model.TableNumber = table.TableNumber;
                    model.Seats = table.Seats;
                }
                return View(model);
            }

            try
            {
                await tableRepository.BookAsync(model.TableId, int.Parse(userId), model.ReservationDateTime);
                TempData["SuccessMessage"] = "Đặt bàn thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var table = await _repository.GetByIdAsync(model.TableId);
                if (table != null)
                {
                    model.TableNumber = table.TableNumber;
                    model.Seats = table.Seats;
                }
                return View(model);
            }
        }
    }
}
