using AMZEnterpriseOrderingSystem.Data;
using AMZEnterpriseOrderingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace AMZEnterpriseOrderingSystem.Controllers.API
{
    [Route("api/[controller]")]
    public class CouponAPIController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CouponAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/<controller>
        [HttpGet]
        public IActionResult Get(double orderTotal, string couponCode = null)
        {
            //Return string will have :E for error and :S for success at the end

            var rtn = "";
            if (couponCode == null)
            {
                rtn = orderTotal + ":E";
                return Ok(rtn);
            }

            var couponFromDb = _context.Coupons.FirstOrDefault(c => c.Name == couponCode);

            if (couponFromDb == null)
            {
                rtn = orderTotal + ":E";
                return Ok(rtn);
            }
            if (couponFromDb.MinimumAmount > orderTotal)
            {
                rtn = orderTotal + ":E";
                return Ok(rtn);
            }

            if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupons.ECouponType.Dollar)
            {
                orderTotal = orderTotal - couponFromDb.Discount;
                rtn = orderTotal + ":S";
                return Ok(rtn);
            }
            else
            {
                if (Convert.ToInt32(couponFromDb.CouponType) == (int)Coupons.ECouponType.Percent)
                {
                    orderTotal = orderTotal - (orderTotal * couponFromDb.Discount / 100);
                    rtn = orderTotal + ":S";
                    return Ok(rtn);
                }
            }
            return Ok();
        }



    }
}
