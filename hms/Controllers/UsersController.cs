using hms.Models;
using hms.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace hms.Controllers
{
    [ApiController]
    [Route("/api/v2/users")]
    public class UsersController(
        UserManager<User> users) : ControllerBase
    {
        private readonly UserManager<User> _users = users;


        /*[HttpGet]
        public async Task<PaginatedResponse<ICollection<User>>> GetAll(int page = 1, int page_size = 10)
        {
            
        }*/
    }
}
