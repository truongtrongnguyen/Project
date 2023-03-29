using BangHang.Areas.Blog.Models;
using BangHang.Models;
using BangHang.Models.Blog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("blog/category/[action]/{id?}")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var qr = _context.Categories
                      .Include(c => c.CategoryChildrent)
                      .Include(c => c.CategoryParent);
            var qr1 = qr.ToList().Where(c => c.CategoryParent == null).ToList();
            return View(qr1);
        }
        private void CreateSelectItem(List<Category> source, List<Category> des, int level)
        {
            string prefix = string.Concat(Enumerable.Repeat("--", level));
            foreach(var item in source)
            {
                des.Add(new Category()
                {
                    Id = item.Id,
                    Title = prefix + item.Title
                });;
                if(item.CategoryChildrent != null)
                {
                    CreateSelectItem(item.CategoryChildrent, des, level + 1);
                }
            }
        }
        private SelectList LoadSelectListItem()
        {
            var qr = _context.Categories
                        .Include(c => c.CategoryChildrent)
                        .Include(c => c.CategoryParent);
            var qr1 = qr.ToList().Where(c => c.CategoryParent == null).ToList();
            var item = new List<Category>();
            item.Insert(0, new Category()
            {
                Id = -1,
                Title = "Không có danh mục"
            });
            CreateSelectItem(qr1, item, 0);
            var selectItem = new SelectList(item, "Id", "Title");
            return selectItem;
        }

        [HttpGet]
        public IActionResult Create()
        {

            var selectItem = LoadSelectListItem();
            ViewData["CategoryParentID"] = selectItem;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel categoryModel)
        {
            if(ModelState.IsValid)
            {
                if(String.IsNullOrEmpty(categoryModel.Slug))
                {
                    categoryModel.Slug = App.Utilities.AppUtilities.GenerateSlug(categoryModel.Title);
                }
                if(_context.Categories.Any(c => c.Slug == categoryModel.Slug))
                {
                    ModelState.AddModelError("Slug", "Phải chọn danh mục khác");
                }
                if(categoryModel.CategoryParentID == -1)
                {
                    categoryModel.CategoryParentID = null;
                }
                var category = new Category()
                {
                    CategoryParentID = categoryModel.CategoryParentID,
                    Title = categoryModel.Title,
                    DesCriptions = categoryModel.DesCriptions,
                    Slug = categoryModel.Slug,
                    SeoTitle = categoryModel.SeoTitle,
                    SeoKeyword = categoryModel.SeoKeyword,
                    SeoDesCriptions = categoryModel.SeoDesCriptions,
                    IsActive = categoryModel.IsActive
                };
                _context.Categories.Add(category);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            var selectItem = LoadSelectListItem();
            ViewData["CategoryParentID"] = selectItem;
            return View(categoryModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories
                            .Where(c => c.Id == id).FirstOrDefault();
            if(category == null)
            {
                return Content("Khong tim thay");
            }
            var selectItem = LoadSelectListItem();
            ViewData["CategoryParentID"] = selectItem;
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category categoryModel)
        {
            var canUpdate = true;
            var category = _context.Categories
                .Where(c => c.Id == categoryModel.Id && c.Id == id).FirstOrDefault();
            if (category == null)
            {
                canUpdate = false;
                return Content("Khong tim thay");
            }
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(categoryModel.Slug))
                {
                    categoryModel.Slug = App.Utilities.AppUtilities.GenerateSlug(categoryModel.Title);
                }
                if (_context.Categories.Any(c => c.Slug == categoryModel.Slug && c.Id != categoryModel.Id))
                {
                    canUpdate = false;
                    ModelState.AddModelError("CategoryParentID", "Phải chọn danh mục khác");
                }
                if (categoryModel.CategoryParentID == categoryModel.Id)
                {
                    ModelState.AddModelError("CategoryParentID", "Phải chọn danh mục khác");
                    canUpdate = false;
                }
               
                if (categoryModel.CategoryParentID != -1 && canUpdate)
                {
                    var categoryChildrent = _context.Categories
                                            .Include(c => c.CategoryChildrent)
                                            .ToList()
                                            .Where(c => c.CategoryParentID == categoryModel.Id);
                    // Func Check CategoryParentID
                    Func<List<Category>, bool> checkCategoryParentID = null;
                    checkCategoryParentID = (List<Category> categorychildrent) =>
                    {
                        foreach (var item in categorychildrent)
                        {
                            if (categoryModel.CategoryParentID == item.Id)
                            {
                                canUpdate = false;
                                ModelState.AddModelError("CategoryparentID", "Phải chọn danhh mục cha khác");
                                return true;
                            }
                            if (item.CategoryChildrent?.Count > 0)
                            {
                                return checkCategoryParentID(item.CategoryChildrent);
                            }
                        }
                        return false;
                    };
                    // Call Fuc
                    checkCategoryParentID(categoryChildrent.ToList());
                }
                if (canUpdate)
                {
                    if (categoryModel.CategoryParentID == -1)
                    {
                        categoryModel.CategoryParentID = null;
                    }
                    category.CategoryParentID = categoryModel.CategoryParentID;
                    category.Title = categoryModel.Title;
                    category.DesCriptions = categoryModel.DesCriptions;
                    category.Slug = categoryModel.Slug;
                    category.SeoTitle = categoryModel.SeoTitle;
                    category.SeoKeyword = categoryModel.SeoKeyword;
                    category.SeoDesCriptions = categoryModel.SeoDesCriptions;
                    category.IsActive = categoryModel.IsActive;

                    _context.Categories.Update(category);
                    _context.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }
            var selectItem = LoadSelectListItem();
            ViewData["CategoryParentID"] = selectItem;
            return View(categoryModel);
        }
        [HttpGet]
        public IActionResult Detail(int id)
        {
            var category = _context.Categories
                            .Where(c => c.Id == id).FirstOrDefault();
            if (category == null)
            {
                return Content("Khong tim thay");
            }
            return View(category);
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories
                            .Where(c => c.Id == id).FirstOrDefault();
            if(category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
