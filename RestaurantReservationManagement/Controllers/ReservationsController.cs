using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using RestaurantBusiness.Models;

namespace FUCourseManagement.Controllers
{
    public class ReservationsController : BaseController<Reservation, int>
    {
        private readonly ITableRepository _tableRepository;
        private readonly IReservationRepository _reservationRepository;

        public ReservationsController(IReservationRepository repository, ITableRepository tableRepository)
            : base(repository)
        {
            _tableRepository = tableRepository;
            _reservationRepository = repository;
        }

        private async Task<bool> IsTableAvailable(int tableId, DateTime reservationDate)
        {
            var table = await _tableRepository.GetByIdAsync(tableId);
            if (table == null || table.Status == "Occupied")
                return false;

            var existingReservations = await _reservationRepository.GetAllAsync();
            return !existingReservations.Any(r => 
                r.TableId == tableId && 
                r.ReservationDate?.Date == reservationDate.Date &&
                r.Status != "Cancelled");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Create(Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại thông tin nhập.";
                return View("~/Views/Shared/Generic/Create.cshtml", reservation);
            }

            if (reservation.ReservationDate == null || reservation.ReservationDate < DateTime.Now)
            {
                ModelState.AddModelError("ReservationDate", "Ngày đặt bàn không hợp lệ");
                TempData["ErrorMessage"] = "Ngày đặt bàn không hợp lệ";
                return View("~/Views/Shared/Generic/Create.cshtml", reservation);
            }

            if (!await IsTableAvailable(reservation.TableId ?? 0, reservation.ReservationDate.Value))
            {
                ModelState.AddModelError("TableId", "Bàn này đã được đặt hoặc không khả dụng");
                TempData["ErrorMessage"] = "Bàn này đã được đặt hoặc không khả dụng";
                return View("~/Views/Shared/Generic/Create.cshtml", reservation);
            }

            try
            {
                reservation.Status = "Pending";
                reservation.CreatedAt = DateTime.Now;

                var table = await _tableRepository.GetByIdAsync(reservation.TableId ?? 0);
                if (table != null)
                {
                    table.Status = "Reserved";
                    await _tableRepository.UpdateAsync(table);
                }

                await _repository.AddAsync(reservation);
                TempData["SuccessMessage"] = "Đặt bàn thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi đặt bàn: {ex.Message}";
                return View("~/Views/Shared/Generic/Create.cshtml", reservation);
            }
        }
    }
}
