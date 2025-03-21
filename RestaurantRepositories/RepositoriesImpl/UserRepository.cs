using Microsoft.EntityFrameworkCore;
using Repositories.Helpers;
using RestaurantBusiness.Models;
using RestaurantDataAccess;

namespace Repositories.RepositoriesImpl
{
    public class UserRepository : IUserRepository
    {
        private UserDAO _dao;

        public UserRepository(UserDAO userDAO)
        {
            _dao = userDAO;
        }

        public async Task<User> AddAsync(User entity)
        {
            entity.Password = PasswordHasher.HashPassword(entity.Password);
            return await _dao.AddAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _dao.DeleteAsync(id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dao.GetAllAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _dao.GetByIdAsync(id);
        }

        public async Task<User?> Login(string email, string password)
        {
            var user = await _dao.GetQueryable().FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;
        }

        public async Task<User> UpdateAsync(User entity)
        {
            return await _dao.UpdateAsync(entity);
        }
    }
}
