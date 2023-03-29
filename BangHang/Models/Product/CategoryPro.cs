using System.ComponentModel.DataAnnotations;

namespace BangHang.Models.Product
{
    public class CategoryPro
    {
        [Key]
        public int? Id { get; set; }

        [Display(Name = "Tên danh mục")]
        [Required(ErrorMessage = "Phải nhập {0}")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
        public string Title { get; set; }

        [Display(Name = "Url")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9]")]
        public string? Slug { get; set; }

        [Display(Name = "Mô tả ngắn")]
        public string? DesCriptions { get; set; }
        public int? CategoryChildrentID { get; set; }
        public List<CategoryPro>? CategoryChildrent { get; set; }
        [Display(Name =  "Danh mục")]
        public int? CategoryParentID { get; set; }
        public CategoryPro? CategoryParent { get; set; }

        [Display(Name = "SeoTitle")]
        public string? SeoTitle { get; set; }
        public string? SeoKeyword { get; set; }
        public string? SeoDesCriptions { get; set; }

        [Display(Name = "Hiển thị")]
        public bool IsActive { get; set; }

		[Display(Name= "Ảnh đại diện")]       
        public string? Image { get; set; }
    }
}
