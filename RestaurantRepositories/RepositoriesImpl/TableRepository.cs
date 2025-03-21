using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> BookAsync(int id, int v, DateTime reservationDate)
        {
            var table = await GetByIdAsync(id);
            if (table == null)
            {
                throw new Exception("Không tìm thấy.");
            }
            if (reservationDate < DateTime.Now)
            {
                throw new Exception("Ngày đặt không được ở quá khứ.");
            }

            var exist = table.Reservations.FirstOrDefault(e =>
                e.UserId == v && e.ReservationDate == reservationDate
            );
            if (exist != null)
            {
                if (!exist.Status.Equals("Cancelled"))
                {
                    exist.Status = "Cancelled";
                    table.Status = "Avaiable";
                    await UpdateAsync(table);
                    await reservationDAO.UpdateAsync(exist);
                    return true;
                }
                else
                {
                    if (table.Status != "Available")
                    {
                        throw new Exception("Bàn không khả dụng để đặt lại.");
                    }
                    exist.Status = "Pending";
                    table.Status = "Reserved";
                    await UpdateAsync(table);
                    await reservationDAO.UpdateAsync(exist);
                    return true;
                }
            }
            else
            {
                var reservation = new Reservation
                {
                    Status = "Pending",
                    TableId = id,
                    UserId = v,
                    ReservationDate = reservationDate,
                };

                await reservationDAO.AddAsync(reservation);
                table.Status = "Reserved";
                await UpdateAsync(table);
                return true;
            }
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
