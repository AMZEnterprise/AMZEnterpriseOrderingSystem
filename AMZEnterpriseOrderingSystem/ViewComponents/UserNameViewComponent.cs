using AMZEnterpriseOrderingSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AMZEnterpriseOrderingSystem.ViewComponents
{
    public class UserNameViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public UserNameViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var userFromDb = await _db.Users.Where(u => u.Id == claim.Value).FirstOrDefaultAsync();

            return View(userFromDb);
        }
    }
}
