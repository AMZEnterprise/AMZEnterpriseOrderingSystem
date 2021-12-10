using AMZEnterpriseOrderingSystem.Data;
using AMZEnterpriseOrderingSystem.Models;
using AMZEnterpriseOrderingSystem.Models.MenuItemViewModels;
using AMZEnterpriseOrderingSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AMZEnterpriseOrderingSystem.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class MenuItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }


        public MenuItemsController(ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _context.Category.ToList(),
                MenuItem = new Models.MenuItem()
            };
        }


        //GET : MenuItems
        public async Task<IActionResult> Index()
        {
            var menuItems = _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory);
            return View(await menuItems.ToListAsync());
        }

        //GET : MenuItems Create
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST : MenuItems Create
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());
            if(!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            _context.MenuItem.Add(MenuItemVM.MenuItem);
            await _context.SaveChangesAsync();

            //Image Being Saved
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await _context.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if(files[0]!=null && files[0].Length>0)
            {
                //when user uploads an image
                var uploads = Path.Combine(webRootPath, "images");
                var extension = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //when user does not upload image
                var uploads = Path.Combine(webRootPath, @"images\"+ SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image= @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        public JsonResult GetSubCategory(int categoryId)
        {
            List<SubCategory> subCategoryList = new List<SubCategory>();

            subCategoryList = (from subCategory in _context.SubCategory
                               where subCategory.CategoryId == categoryId
                               select subCategory).ToList();

            return Json(new SelectList(subCategoryList, "Id", "Name"));
        }

        //GET : Edit MenuItem
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = _context.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();

            if(MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //POST : Edit MenuItems
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (id!= MenuItemVM.MenuItem.Id)
            {
                return NotFound();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    string webRootPath = _hostingEnvironment.WebRootPath;
                    var files = HttpContext.Request.Form.Files;
                    var menuItemFromDb = _context.MenuItem.FirstOrDefault(m => m.Id == MenuItemVM.MenuItem.Id);

                    if(files[0].Length>0 && files[0]!=null)
                    {
                        //if user uploads a new image
                        var uploads = Path.Combine(webRootPath, "images");

                        var extension_New = files[0].FileName.Substring(files[0].FileName.LastIndexOf("."), files[0].FileName.Length - files[0].FileName.LastIndexOf("."));

                        var extension_Old = menuItemFromDb.Image.Substring(menuItemFromDb.Image.LastIndexOf("."), menuItemFromDb.Image.Length - menuItemFromDb.Image.LastIndexOf("."));

                        if(System.IO.File.Exists(Path.Combine(uploads,MenuItemVM.MenuItem.Id+extension_Old)))
                        {
                            System.IO.File.Delete(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_Old));
                        }
                        using (var filestream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_New), FileMode.Create))
                        {
                            await files[0].CopyToAsync(filestream);
                        }
                        MenuItemVM.MenuItem.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_New;
                    }

                    if(MenuItemVM.MenuItem.Image !=null)
                    {
                        menuItemFromDb.Image = MenuItemVM.MenuItem.Image;
                    }
                    menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
                    menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
                    menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
                    menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
                    menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
                    menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {

                }
                return RedirectToAction(nameof(Index));
            }
            MenuItemVM.SubCategory = _context.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToList();
            return View(MenuItemVM); 

        }

        //GET : Details MenuItem
        public async Task<IActionResult> Details(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if(MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //GET : Delete MenuItem
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await _context.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //POST Delete MenuItem
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            MenuItem menuItem = await _context.MenuItem.FindAsync(id);

            if(menuItem!=null)
            {
                var uploads = Path.Combine(webRootPath, "images");
                var extension = menuItem.Image.Substring(menuItem.Image.LastIndexOf(".", StringComparison.Ordinal), menuItem.Image.Length - menuItem.Image.LastIndexOf(".", StringComparison.Ordinal));

                var imagePath = Path.Combine(uploads, menuItem.Id + extension);
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _context.MenuItem.Remove(menuItem);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }


    }
}