﻿using RestaurantBusiness.Models;

namespace Repositories
{
    public interface ITableRepository : IBaseRepository<Table, int>
    {
        Task<bool> BookAsync(int tableId, int userId, DateTime reservationDate);
        Task<bool> CancelBookingAsync(int tableId, int userId);
        Task<bool> CheckStatusAsync(int id, int userId);
    }
}
