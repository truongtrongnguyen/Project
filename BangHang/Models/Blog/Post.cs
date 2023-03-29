using System.ComponentModel.DataAnnotations;

namespace BangHang.Models.Blog
{
    public class Post
    {
        [Key]
        public int? Id { get; set; }

        [Required]
        [Display(Name ="Tiêu đề")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} phải từ {2} đến {1} ký tự")]
        public string Title { get; set; }

        [Display(Name ="Mô tả")]
        [StringLength(150,MinimumLength = 3, ErrorMessage = "{0} phải từ {2} đến {1} ký tự")]
        public string? DesCriptions { get; set; }

        [Display(Name = "Nội dung")]
        [DataType(DataType.Text)]
        public string? Contents { get; set; }

        [Display(Name = "Url")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9]")]
        public string? Slug { get; set; }

        public int? PostCategoryID { get; set; }
        public List<PostCategory>? PostCategory { get; set; }

        public DateTime? DateCreate { get; set; }
        public DateTime? DateUpdate { get; set; }

        [Display(Name = "Hiển thị")]        
        public bool IsActive { get; set; }

        [Display(Name = "Nổi bậc")]
        public bool IsHot { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string? Image { get; set; }

    }
}
