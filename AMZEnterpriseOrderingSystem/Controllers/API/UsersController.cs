using AMZEnterpriseOrderingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace AMZEnterpriseOrderingSystem.Controllers.API
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get(string type, string query = null)
        {
            if (type.Equals("email") && query != null)
            {
                var customerQuery = _context.Users.Where(u => u.Email.ToLower().Contains(query.ToLower()));

                return Ok(customerQuery.ToList());
            }
            return Ok();
        }

    }
}
