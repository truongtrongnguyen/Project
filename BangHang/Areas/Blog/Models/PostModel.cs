using BangHang.Models.Blog;
using System.ComponentModel.DataAnnotations;

namespace BangHang.Areas.Blog.Models
{
    public class PostModel : Post
    {
        // Để cho biết các bài Post nó thuộc Category nào
        [Display(Name = "Thuộc chuyên mục")]
        public int[]? CategoryIDs { get; set; }
        [Display(Name ="Ảnh đại diện")]
        public IFormFile? Avata { get; set; }

    }
}
