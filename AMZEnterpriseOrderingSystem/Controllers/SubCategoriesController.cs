using AMZEnterpriseOrderingSystem.Data;
using AMZEnterpriseOrderingSystem.Models;
using AMZEnterpriseOrderingSystem.Models.SubCategoryViewModels;
using AMZEnterpriseOrderingSystem.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMZEnterpriseOrderingSystem.Controllers
{
    [Authorize(Roles = SD.AdminEndUser)]
    public class SubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;


        [TempData]
        public string StatusMessage { get; set; }


        public SubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        //GET Action 
        public async Task<IActionResult> Index()
        {
            var subCategories = _context.SubCategory.Include(s => s.Category);

            return View(await subCategories.ToListAsync());
        }

        //GET Action for Create
        public IActionResult Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _context.Category.ToList(),
                SubCategory = new SubCategory(),
                SubCategoryList = _context.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToList()
            };

            return View(model);
        }

        //POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doesSubCategoryExists = _context.SubCategory.Count(s => s.Name == model.SubCategory.Name);
                var doesSubCatAndCatExists = _context.SubCategory.Count(s => s.Name == model.SubCategory.Name && s.CategoryId==model.SubCategory.CategoryId);


                if(doesSubCategoryExists > 0 && model.isNew)
                {
                    //error
                    StatusMessage = "خطا : زیر دسته ای با این نام از قبل موجود می باشد";
                }
                else
                {
                    if(doesSubCategoryExists==0 && !model.isNew)
                    {
                        //error 
                        StatusMessage = "خطا : زیر دسته موجود نمی باشد";
                    }
                    else
                    {
                        if(doesSubCatAndCatExists > 0)
                        {
                            //error
                            StatusMessage = "خطا : دسته بندی با این زیر دسته موجود می باشد";
                        }
                        else
                        {
                            _context.Add(model.SubCategory);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _context.Category.ToList(),
                SubCategory = model.SubCategory,
                SubCategoryList = _context.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);

        }



        //GET Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var subCategory = await _context.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _context.Category.ToList(),
                SubCategory = subCategory,
                SubCategoryList = _context.SubCategory.Select(p => p.Name).Distinct().ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,SubCategoryAndCategoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                var doesSubCategoryExists = _context.SubCategory.Count(s => s.Name == model.SubCategory.Name);
                var doesSubCatAndCatExists = _context.SubCategory.Count(s => s.Name == model.SubCategory.Name && s.CategoryId == model.SubCategory.CategoryId);

                if(doesSubCategoryExists == 0)
                {
                    StatusMessage = "خطا : زیر دسته موجود نمی باشد. شما نمی توانید در این محل زیر دسته ایجاد کنید";
                }
                else
                {
                    if(doesSubCatAndCatExists > 0)
                    {
                        StatusMessage = "خطا : دسته بندی با این زیر دسته موجود می باشد";
                    }
                    else
                    {
                        var subCatFromDb = _context.SubCategory.Find(id);
                        subCatFromDb.Name = model.SubCategory.Name;
                        subCatFromDb.CategoryId = model.SubCategory.CategoryId;
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = _context.Category.ToList(),
                SubCategory = model.SubCategory,
                SubCategoryList = _context.SubCategory.Select(p => p.Name).Distinct().ToList(),
                StatusMessage = StatusMessage
            };
            return View(modelVM);
        }



        //GET Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _context.SubCategory.Include(s=>s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }
            
            return View(subCategory);
        }

        //GET Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _context.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        //POST Delete
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _context.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            _context.SubCategory.Remove(subCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

    }
}