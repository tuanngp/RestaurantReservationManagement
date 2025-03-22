﻿﻿﻿using Microsoft.EntityFrameworkCore;
using RestaurantBusiness.Models;
using RestaurantDataAccess;

namespace Repositories.RepositoriesImpl
{
    public class TableRepository : ITableRepository
    {
        private TableDAO _dao;
        private ReservationDAO reservationDAO;

        public TableRepository(TableDAO tableDAO, ReservationDAO reservationDAO)
        {
            _dao = tableDAO;
            this.reservationDAO = reservationDAO;
        }

        public async Task<Table> AddAsync(Table entity)
        {
            bool exists = await _dao.GetQueryable()
                .AnyAsync(t => t.TableNumber == entity.TableNumber);
            if (exists)
            {
                throw new InvalidOperationException(
                    $"TableNumber {entity.TableNumber} already exists."
                );
            }

            return await _dao.AddAsync(entity);
        }

        public async Task<bool> BookAsync(int tableId, int userId, DateTime reservationDate)
        {
            var table = await GetByIdAsync(tableId);
            if (table == null)
            {
                throw new Exception("Không tìm thấy bàn.");
            }

            if (table.Status != "Available")
            {
                throw new Exception("Bàn này không khả dụng để đặt.");
            }

            if (reservationDate < DateTime.Now)
            {
                throw new Exception("Thời gian đặt bàn không được ở quá khứ.");
            }

            // Kiểm tra xem bàn có đang được đặt trong thời gian này không
            var existingReservations = await reservationDAO.GetQueryable()
                .Where(r => r.TableId == tableId && r.Status != "Cancelled")
                .Where(r => r.ReservationDate.HasValue && 
                           r.ReservationDate.Value.Date == reservationDate.Date)
                .ToListAsync();

            if (existingReservations.Any())
            {
                throw new Exception("Bàn đã được đặt trong thời gian này.");
            }

            // Tạo đặt bàn mới
            var reservation = new Reservation
            {
                Status = "Pending",
                TableId = tableId,
                UserId = userId,
                ReservationDate = reservationDate,
                CreatedAt = DateTime.Now
            };

            await reservationDAO.AddAsync(reservation);
            table.Status = "Reserved";
            await UpdateAsync(table);

            return true;
        }

        public async Task<bool> CancelBookingAsync(int tableId, int userId)
        {
            var table = await GetByIdAsync(tableId);
            if (table == null)
            {
                throw new Exception("Không tìm thấy bàn.");
            }

            // Tìm reservation hiện tại của user cho bàn này
            var existingReservation = await reservationDAO.GetQueryable()
                .Where(r => r.TableId == tableId && 
                           r.UserId == userId && 
                           r.Status != "Cancelled")
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefaultAsync();

            if (existingReservation == null)
            {
                throw new Exception("Không tìm thấy đơn đặt bàn.");
            }

            // Hủy đặt bàn
            existingReservation.Status = "Cancelled";
            await reservationDAO.UpdateAsync(existingReservation);

            // Cập nhật trạng thái bàn
            table.Status = "Available";
            await UpdateAsync(table);

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _dao.DeleteAsync(id);
        }

        public async Task<IEnumerable<Table>> GetAllAsync()
        {
            return await _dao.GetAllAsync();
        }

        public async Task<Table?> GetByIdAsync(int id)
        {
            return await _dao.GetByIdAsync(id);
        }

        public async Task<bool> CheckStatusAsync(int id, int userId)
        {
            var table = await GetByIdAsync(id);
            if (table == null || table.Status != "Available")
                return false;

            return true;
        }

        public async Task<Table> UpdateAsync(Table entity)
        {
            return await _dao.UpdateAsync(entity);
        }
    }
}
