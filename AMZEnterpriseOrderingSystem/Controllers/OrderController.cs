using AMZEnterpriseOrderingSystem.Data;
using AMZEnterpriseOrderingSystem.Extensions;
using AMZEnterpriseOrderingSystem.Models;
using AMZEnterpriseOrderingSystem.Models.OrderDetailsViewModel;
using AMZEnterpriseOrderingSystem.Services;
using AMZEnterpriseOrderingSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AMZEnterpriseOrderingSystem.Controllers
{
    public class OrderController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private int PageSize = 2;
        public OrderController(ApplicationDbContext context,IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }


        //CONFIRM GET
        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = _context.OrderHeader.FirstOrDefault(o => o.Id == id && o.UserId == claim.Value),
                OrderDetail = _context.OrderDetails.Where(o => o.OrderId == id).ToList()
            };

            var customerEmail = _context.Users.FirstOrDefault(u => u.Id == orderDetailsViewModel.OrderHeader.UserId)?.Email;
            await _emailSender.SendOrderStatusAsync(customerEmail, orderDetailsViewModel.OrderHeader.Id.ToString(), SD.StatusSubmitted);
            return View(orderDetailsViewModel);
        }


        [Authorize]
        public IActionResult OrderHistory(int productPage=1)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<OrderHeader> orderHeaderList = _context.OrderHeader.Where(u => u.UserId == claim.Value).OrderByDescending(u => u.OrderDate).ToList();

            foreach(OrderHeader item in orderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetail = _context.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                };
                orderListVM.Orders.Add(individual);
            }
            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderBy(p => p.OrderHeader.Id)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count
            };



            return View(orderListVM);
        }

        [Authorize(Roles =SD.AdminEndUser)]
        public IActionResult ManageOrder()
        {
            List<OrderDetailsViewModel> orderDetailsVm = new List<OrderDetailsViewModel>();
            List<OrderHeader> orderHeaderList = _context.OrderHeader.Where(o=>o.Status==SD.StatusSubmitted || o.Status==SD.StatusInProcess)
                .OrderByDescending(u => u.PickUpTime).ToList();

            foreach (OrderHeader item in orderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetail = _context.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                };
                orderDetailsVm.Add(individual);

            }
            return View(orderDetailsVm);
        }



        [Authorize(Roles =SD.AdminEndUser)]
        public async Task<IActionResult> OrderPrepare(int orderId)
        {
            OrderHeader orderHeader = await _context.OrderHeader.FindAsync(orderId);
            orderHeader.Status = SD.StatusInProcess;
            await _context.SaveChangesAsync();
            return RedirectToAction("ManageOrder", "Order");

        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderReady(int orderId)
        {
            OrderHeader orderHeader = await _context.OrderHeader.FindAsync(orderId);
            orderHeader.Status = SD.StatusReady;
            await _context.SaveChangesAsync();
            var customerEmail = _context.Users.FirstOrDefault(u => u.Id == orderHeader.UserId)?.Email;
            await _emailSender.SendOrderStatusAsync(customerEmail, orderHeader.Id.ToString(), SD.StatusReady);
            return RedirectToAction("ManageOrder", "Order");

        }

        [Authorize(Roles = SD.AdminEndUser)]
        public async Task<IActionResult> OrderCancel(int orderId)
        {
            OrderHeader orderHeader = await _context.OrderHeader.FindAsync(orderId);
            orderHeader.Status = SD.StatusCancelled;
            await _context.SaveChangesAsync();
            var customerEmail = _context.Users.FirstOrDefault(u => u.Id == orderHeader.UserId)?.Email;
            await _emailSender.SendOrderStatusAsync(customerEmail, orderHeader.Id.ToString(), SD.StatusCancelled);
            return RedirectToAction("ManageOrder", "Order");

        }

        //GET Order Pickup
        public IActionResult OrderPickup(string searchEmail=null,string searchPhone=null,string searchOrder=null)
        {
            List<OrderDetailsViewModel> orderDetailsVm = new List<OrderDetailsViewModel>();

            if (searchEmail != null || searchPhone != null || searchOrder != null)
            {
                //filtering the criteria
                var user = new ApplicationUser();
                List<OrderHeader> orderHeaderList = new List<OrderHeader>();

                if(searchOrder!=null)
                {
                    orderHeaderList = _context.OrderHeader.Where(o => o.Id == Convert.ToInt32(searchOrder)).ToList();
                }
                else
                {
                    if(searchEmail!=null)
                    {
                        user = _context.Users.FirstOrDefault(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            user = _context.Users.FirstOrDefault(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
                        }
                    }
                }
                if(user!=null || orderHeaderList.Count>0)
                {
                    if(orderHeaderList.Count==0)
                    {
                        orderHeaderList = _context.OrderHeader.Where(o => o.UserId == user.Id).OrderByDescending(o => o.OrderDate).ToList();
                    }
                    foreach(OrderHeader item in orderHeaderList)
                    {
                        OrderDetailsViewModel individual = new OrderDetailsViewModel
                        {
                            OrderHeader = item,
                            OrderDetail = _context.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                        };
                        orderDetailsVm.Add(individual);
                    }
                }


            }
            else
            {
                List<OrderHeader> orderHeaderList = _context.OrderHeader.Where(o => o.Status == SD.StatusReady)
                    .OrderByDescending(u => u.PickUpTime).ToList();

                foreach (OrderHeader item in orderHeaderList)
                {
                    OrderDetailsViewModel individual = new OrderDetailsViewModel
                    {
                        OrderHeader = item,
                        OrderDetail = _context.OrderDetails.Where(o => o.OrderId == item.Id).ToList()
                    };
                    orderDetailsVm.Add(individual);

                }
            }
            return View(orderDetailsVm);
        }


        [Authorize(Roles =SD.AdminEndUser)]
        public IActionResult OrderPickupDetails(int orderId)
        {
            OrderDetailsViewModel orderDetailsVm = new OrderDetailsViewModel
            {
                OrderHeader = _context.OrderHeader.FirstOrDefault(o => o.Id == orderId)
            };
            orderDetailsVm.OrderHeader.ApplicationUser = _context.Users.FirstOrDefault(u => u.Id == orderDetailsVm.OrderHeader.UserId);
            orderDetailsVm.OrderDetail = _context.OrderDetails.Where(o => o.OrderId == orderDetailsVm.OrderHeader.Id).ToList();

            return View(orderDetailsVm);
        }

        [HttpPost]
        [Authorize(Roles =SD.AdminEndUser)]
        [ActionName("OrderPickupDetails")]
        public async Task<IActionResult> OrderPickupDetailsPost(int orderId)
        {
            OrderHeader orderHeader = await _context.OrderHeader.FindAsync(orderId);
            orderHeader.Status = SD.StatusCompleted;
            await _context.SaveChangesAsync();
            return RedirectToAction("OrderPickup", "Order");

        }


        public IActionResult OrderSummaryExport(int orderId)
        {
            return View();
        }
        [HttpPost]
        public IActionResult OrderSummaryExport(OrderExportViewModel orderExportVM)
        {
            List<OrderHeader> orderHeaderList = _context.OrderHeader.Where(o => o.OrderDate >= orderExportVM.startDate && o.OrderDate <= orderExportVM.endDate).ToList();
            List<OrderDetails> orderDetailList = new List<OrderDetails>();
            List<OrderDetails> IndividualOrderList = new List<OrderDetails>();
            foreach (var orderHeader in orderHeaderList)
            {
                IndividualOrderList = _context.OrderDetails.Where(o => o.OrderId == orderHeader.Id).ToList();

                foreach (var individualOrder in IndividualOrderList)
                {
                    orderDetailList.Add(individualOrder);
                }
            }

            byte[] bytes = Encoding.ASCII.GetBytes(ConvertToString(orderDetailList));
            return File(bytes, "application/text", "OrderDetail.csv");
        }

        public String ConvertToString<T>(IList<T> data)
        {

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }
            table.Columns.Remove("OrderHeader");
            table.Columns.Remove("MenuItemId");
            table.Columns.Remove("MenuItem");
            table.Columns.Remove("Description");

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in table.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }
            return sb.ToString();
        }

    }
}