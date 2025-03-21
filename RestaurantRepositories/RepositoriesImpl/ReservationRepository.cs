using RestaurantBusiness.Models;
using RestaurantDataAccess;

namespace Repositories.RepositoriesImpl
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly ReservationDAO _ReservationDAO;

        public ReservationRepository(ReservationDAO reservationDAO)
        {
            _ReservationDAO = reservationDAO;
        }

        public async Task<Reservation> AddAsync(Reservation entity)
        {
            return await _ReservationDAO.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _ReservationDAO.DeleteAsync(id);
        }

        public async Task<IEnumerable<Reservation>> GetAllAsync()
        {
            return await _ReservationDAO.GetAllAsync();
        }

        public async Task<Reservation?> GetByIdAsync(int id)
        {
            return await _ReservationDAO.GetByIdAsync(id);
        }

        public async Task<Reservation> UpdateAsync(Reservation entity)
        {
            return await _ReservationDAO.UpdateAsync(entity);
        }
    }
}
