using App.Utilities;
using BangHang.Areas.Blog.Models;
using BangHang.Models;
using BangHang.Models.Blog;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BangHang.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("/blog/post/[action]/{id?}")]
    [Authorize(Roles = "AdminManager")]
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        public PostController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var posts = _context.Posts
                        .Include(p => p.PostCategory)
                        .ThenInclude(p => p.Category)
                        .OrderByDescending(x => x.DateCreate)
                        .ToList();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var selctList = new MultiSelectList(categories, "Id", "Title");
            ViewBag.Categories = selctList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Slug,IsActive,IsHot,Avata,DesCriptions,Contents,CategoryIDs")] PostModel postModel)
        {
            if (ModelState.IsValid)
            {
                if (postModel.Slug == null)
                {
                    postModel.Slug = AppUtilities.GenerateSlug(postModel.Title);
                }
                if (_context.Posts.Any(p => p.Slug == postModel.Slug))
                {
                    ModelState.AddModelError("Slug", "Phải chọn danh mục khác");
                }
                var avataName = "";
                if (postModel.Avata != null)
                {
                    avataName = Path.GetFileName(Path.GetRandomFileName())
                           + Path.GetExtension(postModel.Avata.FileName);
                }
                var file = Path.Combine("Uploads", "Posts", avataName);

                var post = new Post();
                post.Title = postModel.Title;
                post.Slug = postModel.Slug;
                post.DesCriptions = postModel.DesCriptions;
                post.Contents = postModel.Contents;
                post.DateCreate = DateTime.Now;
                post.IsActive = postModel.IsActive;
                post.IsHot = postModel.IsHot;
                post.Image = avataName;
                await _context.Posts.AddAsync(post);

                // Tạo quan hệ giữa Category và Post
                if (postModel.CategoryIDs == null)
                {
                    postModel.CategoryIDs = new int[] { }; 
                }
                foreach (var categoryID in postModel.CategoryIDs)
                {
                    _context.PostCategory.Add(
                        new PostCategory()
                        {
                            CategoryID = categoryID,
                            Post = post
                        }
                   );
                }
                _context.SaveChanges();
                if(postModel.Avata != null)
                {
                    using (var streamFile = new FileStream(file, FileMode.Create))
                    {
                        await postModel.Avata.CopyToAsync(streamFile);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var categories = _context.Categories.ToList();
            var selctList = new MultiSelectList(categories, "Id", "Title");
            ViewBag.Categories = selctList;
            return View(postModel);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var categories = _context.Categories.ToList();
            var selctList = new MultiSelectList(categories, "Id", "Title");
            ViewBag.Categories = selctList;

            var post = _context.Posts
                    .Include(p => p.PostCategory)
                    .Where(p => p.Id == id).FirstOrDefault();
            if(post == null)
            {
                return Content("Khong tim thay bai viet");
            }
            var postModel = new PostModel()
            {
                Title = post.Title,
                Slug = post.Slug,
                DesCriptions = post.DesCriptions,
                Contents = post.Contents,
                IsActive = post.IsActive,
                IsHot = post.IsHot,
                Image = post.Image,
                CategoryIDs =  post.PostCategory.Select(c => c.CategoryID.GetValueOrDefault()).ToArray(),
                Id = post.Id
            };
            return View(postModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit (int id, [Bind("CategoryIDs,Title,Slug,IsActive,IsHot,Avata,DesCriptions,Contents,Image,Id")]PostModel postModel)
        {
            if(postModel == null)
            {
                return Content("Noi dung khong hop le");
            }
            var canUpdate = true;
            var post = _context.Posts
                        .Include(p => p.PostCategory)
                        .ThenInclude(p => p.Category)
                        .Where(p => p.Id == id && p.Id == postModel.Id)
                        .FirstOrDefault();
            if(post == null)
            {
                canUpdate = false;
                return Content("Khong tim thay bai viet");
            }
            if (ModelState.IsValid && canUpdate)
            {
                if(String.IsNullOrEmpty(postModel.Slug))
                {
                    postModel.Slug = AppUtilities.GenerateSlug(postModel.Title);
                }
                if(_context.Posts.Any(p => p.Slug == postModel.Slug && p.Id != postModel.Id))
                {
                    canUpdate = false;
                    ModelState.AddModelError("Slug", "Phải chọn danh mục khác");
                }
                if (postModel.Avata != null)
                {
                    // Trường hợp khi create mà không thêm ảnh thì nó sẽ không tiến hành hành vi xóa
                    if(!String.IsNullOrEmpty(post.Image))
                    { 
                        var UrlDeleteAvata = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Posts", post.Image);
                        System.IO.File.Delete(UrlDeleteAvata);
                    }
                     var avataName = Path.GetRandomFileName()
                                  + Path.GetExtension(postModel.Avata.FileName);
                    var file = Path.Combine("Uploads", "Posts", avataName);
                    using (var streamFile = new FileStream(file, FileMode.Create))
                    {
                        await postModel.Avata.CopyToAsync(streamFile);
                    }
                    post.Image = avataName;
                }
                if(postModel.CategoryIDs == null)
                {
                    postModel.CategoryIDs = new int[] {};
                }
                var OldCategoryID = post.PostCategory.Select(p => p.CategoryID).ToArray();
                var NewCatgoryID = postModel.CategoryIDs;
                var RemoveCategoryID = post.PostCategory.Where(p => !NewCatgoryID.Contains(p.CategoryID.GetValueOrDefault()));
                var AddCategoryID = postModel.CategoryIDs.Where(p => !OldCategoryID.Contains(p));

                _context.PostCategory.RemoveRange(RemoveCategoryID);
                foreach (var item in AddCategoryID.ToList())
                {
                    _context.PostCategory.Add(new PostCategory()
                    {
                        CategoryID = item,
                        Post = post
                    });
                }

                post.Title = postModel.Title;
                post.Slug = postModel.Slug;
                post.DesCriptions = postModel.DesCriptions;
                post.Contents = postModel.Contents;
                post.IsActive = postModel.IsActive;
                post.IsHot = postModel.IsHot;
                post.DateUpdate = DateTime.Now;

                _context.Posts.Update(post);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            var categories = _context.Categories.ToList();
            var selctList = new MultiSelectList(categories, "Id", "Title");
            ViewBag.Categories = selctList;
            return View(postModel);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var post = _context.Posts
                        .Include(p => p.PostCategory)
                        .Where(p => p.Id == id)
                        .FirstOrDefault();
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var post = _context.Posts
                    .Include(p => p.PostCategory)
                    .ThenInclude(p => p.Category)
                    .Where(p => p.Id == id)
                    .FirstOrDefault();
            if (post == null)
            {
                return Content("Khong tim thay bai viet");
            }
            return View(post);
        }
    }
}
