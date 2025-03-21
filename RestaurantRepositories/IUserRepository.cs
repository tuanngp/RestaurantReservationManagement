using RestaurantBusiness.Models;

namespace Repositories
{
    public interface IUserRepository : IBaseRepository<User, int>
    {
        Task<User?> Login(string email, string password);
    }
}
