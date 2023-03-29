using App.Utilities;
using BangHang.Areas.Products.CategoryProModels;
using BangHang.Models;
using BangHang.Models.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Areas.Products.Controllers
{
    [Area("Products")]
    [Route("/products/categorypro/[action]/{id?}")]
    public class CategoryProController : Controller
    {
        private readonly AppDbContext _context;
        public const string folderImage = "CategoryPro";
        public CategoryProController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]   
        public IActionResult Index()
        {
            var qr = _context.CategoriesPro
                      .Include(c => c.CategoryChildrent)
                      .Include(c => c.CategoryParent);
            var qr1 = qr.ToList().Where(c => c.CategoryParent == null).ToList();
            ViewBag.folderImage = folderImage;
            return View(qr1);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var selectList = LoadCategory();
            ViewBag.CategoryParentID = selectList;
            return View();
        }
        private void CreateSelectList(List<CategoryPro> source, List<CategoryPro> des, int level)
        {
            string prefex = String.Concat(Enumerable.Repeat("--", level));
            foreach(var item in source)
            {
                des.Add(new CategoryPro()
                {
                    Id = item.Id,
                    Title = prefex + item.Title
                });
                if(item.CategoryChildrent?.Count > 0)
                {
                    CreateSelectList(item.CategoryChildrent, des, level + 1);
                }
            }
        }
        private SelectList LoadCategory()
        {
            var qr = _context.CategoriesPro
                       .Include(c => c.CategoryChildrent)
                       .Include(c => c.CategoryParent);
            var qr1 = qr.ToList().Where(c => c.CategoryParent == null).ToList();
            var items = new List<CategoryPro>();
            items.Insert(0, new CategoryPro()
            {
                Id = -1,
                Title = "Không có danh mục"
            });
            CreateSelectList(qr1, items, 0);
            var selectList = new SelectList(items, "Id", "Title");
            return selectList;
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("CategoryParentID,Title,Slug,IsActive,DesCriptions,SeoTitle,SeoKeyword,SeoDesCriptions,Avata")]CategoryProModel categoryModel)
        {
            if (ModelState.IsValid)
            {
                if(String.IsNullOrEmpty(categoryModel.Slug))
                {
                    categoryModel.Slug = AppUtilities.GenerateSlug(categoryModel.Title);
                }
                if (_context.CategoriesPro.Any(c => c.Slug == categoryModel.Slug))
                {
                    ModelState.AddModelError("Slug", "Phải chọn Url khác");
                }
                if(categoryModel.CategoryParentID == -1)
                {
                    categoryModel.CategoryParentID = null;
                }
                var fileName = "";
                if (categoryModel.Avata != null)
				{
                    // check folder exists, if folder not exists for create new folder
                    string directoryFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderImage);
                    if(!Directory.Exists(directoryFolder))
                    {
                        Directory.CreateDirectory(directoryFolder);
                    }
                    fileName = Path.GetRandomFileName()
                                    + Path.GetExtension(categoryModel.Avata.FileName);
                    var file = Path.Combine(Directory.GetCurrentDirectory(),"Uploads", folderImage, fileName);
                    using (var fileStream = new FileStream(file, FileMode.Create))
					{
                        await categoryModel.Avata.CopyToAsync(fileStream);
					}
				}
                var categoryPro = new CategoryPro()
                {
                    CategoryParentID = categoryModel.CategoryParentID,
                    Title = categoryModel.Title,
                    Slug = categoryModel.Slug,
                    IsActive = categoryModel.IsActive,
                    DesCriptions = categoryModel.DesCriptions,
                    SeoTitle = categoryModel.SeoTitle,
                    SeoKeyword = categoryModel.SeoKeyword,
                    SeoDesCriptions = categoryModel.SeoDesCriptions,
                    Image = fileName
                };
                _context.CategoriesPro.Add(categoryPro);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            var selectList = LoadCategory();
            ViewBag.CategoryParentID = selectList;
            return View(categoryModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var categoryPro = _context.CategoriesPro.FirstOrDefault(c => c.Id == id);
            if(categoryPro == null)
            {
                return NotFound();
            }
            var categoryProModel = new CategoryProModel()
            {
                Title = categoryPro.Title,
                Slug = categoryPro.Slug,
                IsActive = categoryPro.IsActive,
                DesCriptions = categoryPro.DesCriptions,
                SeoTitle = categoryPro.SeoTitle,
                SeoKeyword = categoryPro.SeoKeyword,
                SeoDesCriptions = categoryPro.SeoDesCriptions,
                CategoryParentID = categoryPro.CategoryParentID,
                Image = categoryPro.Image
        };

            var selectList = LoadCategory();
            ViewBag.CategoryParentID = selectList;
            ViewBag.folderImage = folderImage;
            return View(categoryProModel);              
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryParentID,Title,Slug,IsActive,DesCriptions,SeoTitle,SeoKeyword,SeoDesCriptions,Avata")] CategoryProModel categoryModel)
        {
            var categoryPro = _context.CategoriesPro
                              .Include(c => c.CategoryChildrent)
                              .Where(c => c.Id == id && c.Id == categoryModel.Id)
                              .FirstOrDefault();
            if(categoryPro == null)
            {
                return NotFound();
            }
            var canUpdate = true;
            if (ModelState.IsValid)
            {

                if (String.IsNullOrEmpty(categoryModel.Slug))
                {
                    categoryModel.Slug = AppUtilities.GenerateSlug(categoryModel.Title);
                }
                if (_context.CategoriesPro.Any(c => c.Slug == categoryModel.Slug && c.Id != categoryModel.Id))
                {
                    canUpdate = false;
                    ModelState.AddModelError("CategoryParentID", "Phải chọn Url khác");
                }
                if(categoryModel.CategoryParentID == categoryModel.Id)
                {
                    canUpdate = false;
                    ModelState.AddModelError("CategoryParentID", "Phải chọn danh mục khác");
                }
                if(categoryModel.CategoryParentID != null && canUpdate && categoryPro.CategoryChildrent?.Count > 0)
                {
                    var categoryChildrent = _context.CategoriesPro
                                          .Include(c => c.CategoryChildrent)
                                          .ToList()
                                          .Where(c => c.CategoryParentID == categoryModel.Id);
                    Func<List<CategoryPro>, bool> checkCategory = null;
                    checkCategory = (List<CategoryPro> categoryChildrent) =>
                    {
                        foreach(var item in categoryChildrent)
                        {
                            if(item.Id == categoryModel.CategoryParentID)
                            {
                                canUpdate = false;
                                ModelState.AddModelError("CategoryParentID", "Phải chọn danh mục khác");
                                return true;
                            }
                            if(item.CategoryChildrent?.Count > 0)
                            {
                                checkCategory(item.CategoryChildrent);
                            }
                        }
                        return false;
                    };
                    // Call
                    checkCategory(categoryChildrent.ToList());
                }
                if (canUpdate)
                {
					
                    if(categoryModel.Avata != null)
					{
                        if (categoryPro.Image != null)
                        {
                            var pathDelete = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderImage, categoryPro.Image);
                            System.IO.File.Delete(pathDelete);
                        }
                        var fileName = Path.GetRandomFileName()
                                        + Path.GetExtension(categoryModel.Avata.FileName);
                        var file = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderImage, fileName);
                        using (var fileStream = new FileStream(file, FileMode.Create))
						{
                            await categoryModel.Avata.CopyToAsync(fileStream);
						}
                        categoryPro.Image = fileName;
					}
                    if(categoryModel.CategoryParentID == -1)
                    {
                        categoryModel.CategoryParentID = null;
                    }
                    categoryPro.Title = categoryModel.Title;
                    categoryPro.Slug = categoryModel.Slug;
                    categoryPro.IsActive = categoryModel.IsActive;
                    categoryPro.DesCriptions = categoryModel.DesCriptions;
                    categoryPro.SeoKeyword = categoryModel.SeoKeyword;
                    categoryPro.SeoTitle = categoryModel.SeoTitle;
                    categoryPro.SeoDesCriptions = categoryModel.SeoDesCriptions;
                    categoryPro.CategoryParentID = categoryModel.CategoryParentID;

                    _context.CategoriesPro.Update(categoryPro);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            var selectList = LoadCategory();
            ViewBag.CategoryParentID = selectList;
            return View(categoryModel);
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var categoryPro = _context.CategoriesPro
                              .Include(c => c.CategoryChildrent)
                              .Where(c => c.Id == id)
                              .FirstOrDefault();
            return View(categoryPro);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var category = _context.CategoriesPro
                            .Where(c => c.Id == id).FirstOrDefault();
            if (category != null)
            {
                _context.CategoriesPro.Remove(category);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
}
