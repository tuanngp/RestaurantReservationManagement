using RestaurantBusiness.Models;

namespace Repositories
{
    public interface ITableRepository : IBaseRepository<Table, int>
    {
        Task<bool> BookAsync(int id, int v, DateTime reservationDate);
        Task<bool> CheckStatusAsync(int id, int userId);
    }
}
