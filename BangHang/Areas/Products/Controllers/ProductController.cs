using App.Utilities;
using BangHang.Models;
using BangHang.Models.Product;
using BangHang.Product.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Areas.Controllers
{
    [Area("Products")]
    [Route("/products/product/[action]/{id?}")]
    [Authorize(Roles = "AdminManager")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;
        public const string folderImage = "Products";
        public ProductController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.ProductPhoto)
                .Include(p => p.ProductCategory)
                .ThenInclude(p => p.Category)
                .OrderByDescending(p => p.DateCreate)
                .ToList();
            ViewBag.folderImage = folderImage;
            return View(products);
        }
        private MultiSelectList LoadCategory()
        {
            var categoryPro = _context.CategoriesPro.ToList();
            var selectList = new MultiSelectList(categoryPro, "Id", "Title");
            return selectList;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.CategoryPro = LoadCategory();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductModel productModel)
        {
            if(ModelState.IsValid)
            {
                if(String.IsNullOrEmpty(productModel.Slug))
                {
                    productModel.Slug = AppUtilities.GenerateSlug(productModel.Title ?? "");
                }
                if(_context.Products.Any(p => p.Slug == productModel.Slug))
                {
                    ModelState.AddModelError("Slug", "Phải chọn danh mục khác");
                }
                var product = new BangHang.Models.Product.Products()
                {
                    Title = productModel.Title,
                    DesCriptions = productModel.DesCriptions,
                    Contents = productModel.Contents,
                    Slug = productModel.Slug,
                    Price = productModel.Price,
                    PriceSale = productModel.PriceSale,
                    Quantity = productModel.Quantity,
                    Color = productModel.Color,
                    TypeProduct = productModel.TypeProduct,
                    DateCreate = DateTime.Now,
                    IsActive = productModel.IsActive,
                    IsSale = productModel.IsSale,
                    IsHome = productModel.IsHome,
                    SeoTitle = productModel.SeoTitle,
                    SeoKeyword = productModel.SeoKeyword,
                    SeoDesCriptions = productModel.SeoDesCriptions,
                    Sold = productModel.Sold,
                    originalPrice = productModel.originalPrice

                };
                if(productModel.ProductFiles != null)
                {
                    foreach(var i in productModel.ProductFiles)
                    {
                        var fileName = Path.GetRandomFileName()
                                    + Path.GetExtension(i.FileName);
                        var file = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderImage, fileName);
                        using (var streamFile = new FileStream(file, FileMode.Create))
                        {
                            await i.CopyToAsync(streamFile);
                        }
                        _context.ProductPhotos.Add(new ProductPhoto()
                        {
                            Product = product,
                            Name = fileName,
                            UrlName = file
                        });
                    }
                }
                if(productModel.CategoryIDs != null)
                {
                    foreach(var i in productModel.CategoryIDs)
                    {
                        _context.ProductCategories.Add(new ProductCategory()
                        {
                            CategoryID = i,
                            Product = product
                        });
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryPro = LoadCategory();
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products
                        .Include(p => p.ProductPhoto)
                        .Include(p => p.ProductCategory)
                        .ThenInclude(pc => pc.Category)
                        .Where(c => c.Id == id).FirstOrDefault();
            if(product != null)
            {
                var productModel = new ProductModel()
                {
                    Title = product.Title,
                    DesCriptions = product.DesCriptions,
                    Contents = product.Contents,
                    Slug = product.Slug,
                    Price = product.Price,
                    PriceSale = product.PriceSale,
                    Quantity = product.Quantity,
                    Color = product.Color,
                    TypeProduct = product.TypeProduct,
                    IsActive = product.IsActive,
                    IsSale = product.IsSale,
                    IsHome = product.IsHome,
                    SeoTitle = product.SeoTitle,
                    SeoKeyword = product.SeoKeyword,
                    SeoDesCriptions = product.SeoDesCriptions,
                    ProductPhoto = product.ProductPhoto,
                    CategoryIDs = product.ProductCategory.Select(c => c.Category.Id.GetValueOrDefault()).ToList(),
                    Sold = product.Sold,
                    originalPrice = product.originalPrice
                };
                 
                ViewBag.CategoryPro = LoadCategory();
                ViewBag.folderProduct = folderImage;
                return View(productModel);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductModel productModel)
        {
            var canUpdate = true;
            var product = _context.Products
                    .Include(c => c.ProductCategory)
                    .ThenInclude(c => c.Category)
                    .Include(c => c.ProductPhoto)
                    .Where(p => p.Id == id && p.Id == productModel.Id)
                    .FirstOrDefault();
            if(product == null)
            {
                canUpdate = false;
                return NotFound();
            }
            if(ModelState.IsValid && canUpdate)
            {
                if(String.IsNullOrEmpty(productModel.Slug))
                {
                    productModel.Slug = AppUtilities.GenerateSlug(productModel.Title ?? "");
                }
                if(_context.Products.Any(p => p.Slug == productModel.Slug && p.Id != productModel.Id))
                {
                    canUpdate = false;
                    ModelState.AddModelError("Slug", "Phải chọn Url khác");
                }
                if(productModel.ProductFiles?.Count > 0 && canUpdate)
                {
                    foreach( var item in productModel.ProductFiles)
                    {
                        var fileName = Path.GetRandomFileName()
                                    + Path.GetExtension(item.FileName);
                        var file = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", folderImage, fileName);
                        using (var streamFile = new FileStream(file, FileMode.Create))
                        {
                            await item.CopyToAsync(streamFile);
                        }
                        await _context.ProductPhotos.AddAsync(new ProductPhoto()
                        {
                            ProductID = product.Id,
                            Name = fileName,
                            UrlName = file
                        });
                    }
                }
                if(productModel.CategoryIDs?.Count > 0 && canUpdate)
                {
                    var OldCategory = product.ProductCategory.Select(c => c.CategoryID);
                    var NewCategory = productModel.CategoryIDs;
                    var RemoveCategory = from c in product.ProductCategory
                                        .Where(c => !NewCategory.Contains(c.CategoryID.GetValueOrDefault()))
                                         select c;
                    var AddCategory = (from c in NewCategory
                                      where !OldCategory.Contains(c)
                                      select c).ToList();
                    _context.ProductCategories.RemoveRange(RemoveCategory);    
                    foreach(var item in AddCategory)
                    {
                        _context.ProductCategories.Add(new ProductCategory()
                        {
                            CategoryID = item,
                            ProductID = product.Id
                        });
                    }
                    product.Title = productModel.Title;
                    product.DesCriptions = productModel.DesCriptions;
                    product.Contents = productModel.Contents;
                    product.Slug = productModel.Slug;
                    product.Price = productModel.Price;
                    product.PriceSale = productModel.PriceSale;
                    product.Quantity = productModel.Quantity;
                    product.Color = productModel.Color;
                    product.TypeProduct = productModel.TypeProduct;
                    product.DateUpdate = DateTime.Now;
                    product.IsActive = productModel.IsActive;
                    product.IsSale = productModel.IsSale;
                    product.IsHome = productModel.IsHome;
                    product.SeoTitle = productModel.SeoTitle;
                    product.SeoKeyword = productModel.SeoKeyword;
                    product.SeoDesCriptions = productModel.SeoDesCriptions;
                    product.Sold = productModel.Sold;
                    product.DateUpdate = DateTime.Now;
                    product.originalPrice = productModel.originalPrice;
                }
                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            LoadCategory();
            return View(productModel);
        }

        [HttpPost]
        public IActionResult DeleteImage(string ids)
        {
            var strImage = ids.Split(",");
            if(strImage.Length > 0)
            {
                foreach(var i in strImage)
                {
                    var productphoto = _context.ProductPhotos.Find(Convert.ToInt32(i));
                    if(productphoto != null)
                    {
                        _context.ProductPhotos.Remove(productphoto);
                        System.IO.File.Delete(productphoto.UrlName?? "");
                    }                          
                }
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var product = _context.Products
                        .Where(p => p.Id == id)
                        .FirstOrDefault();
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var product = _context.Products
                        .Include(p => p.ProductPhoto)
                        .Include(p => p.ProductCategory)
                        .ThenInclude(pc => pc.Category)
                        .Where(c => c.Id == id).FirstOrDefault();
            if (product != null)
            {
                ViewBag.folderImage = folderImage;
                return View(product);
            }
            return NotFound();
        }

    }
}

