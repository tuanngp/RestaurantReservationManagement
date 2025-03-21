using DataAccessObjects;
using FUBusiness.Models;
using Microsoft.EntityFrameworkCore;
using RestaurantBusiness.Models;

namespace RestaurantDataAccess
{
    public class ReservationDAO : BaseDAO<Reservation, int>
    {
        protected override IQueryable<Reservation> AddIncludes(IQueryable<Reservation> query)
        {
            return query.Include(a => a.Table).Include(a => a.User);
        }
    }
}
