using Microsoft.AspNetCore.Authorization;
using Repositories;
using RestaurantBusiness.Models;

namespace FUCourseManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : BaseController<User, int>
    {
        public UsersController(IUserRepository repository)
            : base(repository) { }
    }
}
