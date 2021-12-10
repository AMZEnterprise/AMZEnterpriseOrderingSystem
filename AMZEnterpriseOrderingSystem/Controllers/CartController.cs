using AMZEnterpriseOrderingSystem.Data;
using AMZEnterpriseOrderingSystem.Models;
using AMZEnterpriseOrderingSystem.Models.OrderDetailsViewModel;
using AMZEnterpriseOrderingSystem.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;

namespace AMZEnterpriseOrderingSystem.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public OrderDetailsCart DetailCart { get; set; }

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            DetailCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };

            DetailCart.OrderHeader.OrderTotal = 0;
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _context.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                DetailCart.listCart = cart.ToList();
            }

            foreach (var list in DetailCart.listCart)
            {
                list.MenuItem = _context.MenuItem.FirstOrDefault(m => m.Id == list.MenuItemId);
                DetailCart.OrderHeader.OrderTotal = DetailCart.OrderHeader.OrderTotal + (list.MenuItem.Price * list.Count);

                if (list.MenuItem.Description.Length > 100)
                {
                    list.MenuItem.Description = list.MenuItem.Description.Substring(0, 99) + "...";
                }
            }

            DetailCart.OrderHeader.PickUpTime = DateTime.Now;

            return View(DetailCart);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            DetailCart.listCart = _context.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToList();

            DetailCart.OrderHeader.OrderDate = DateTime.Now;
            DetailCart.OrderHeader.UserId = claim.Value;
            DetailCart.OrderHeader.Status = SD.StatusSubmitted;
            OrderHeader orderHeader = DetailCart.OrderHeader;
            _context.OrderHeader.Add(orderHeader);
            _context.SaveChanges();

            foreach (var item in DetailCart.listCart)
            {
                item.MenuItem = _context.MenuItem.FirstOrDefault(m => m.Id == item.MenuItemId);
                OrderDetails orderDetails = new OrderDetails
                {
                    MenuItemId = item.MenuItemId,
                    OrderId = orderHeader.Id,
                    Description = item.MenuItem.Description,
                    Name = item.MenuItem.Name,
                    Price = item.MenuItem.Price,
                    Count = item.Count
                };
                _context.OrderDetails.Add(orderDetails);
            }

            _context.ShoppingCart.RemoveRange(DetailCart.listCart);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("CartCount", 0);

            return RedirectToAction("Confirm", "Order", new { id = orderHeader.Id });


        }








        public IActionResult Plus(int cartId)
        {
            var cart = _context.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefault();
            cart.Count += 1;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _context.ShoppingCart.Where(c => c.Id == cartId).FirstOrDefault();
            if (cart.Count == 1)
            {
                _context.ShoppingCart.Remove(cart);
                _context.SaveChanges();

                var cnt = _context.ShoppingCart.Where(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32("CartCount", cnt);
            }
            else
            {
                cart.Count -= 1;
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}