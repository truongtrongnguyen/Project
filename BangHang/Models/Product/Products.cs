using System.ComponentModel.DataAnnotations;

namespace BangHang.Models.Product
{
    public class Products
    {
        [Key]
        public int? Id { get; set; }

        [Required(ErrorMessage = "Phải nhập {0}")]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
        public string? Title { get; set; }

        [Display(Name = "Url")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Chỉ dùng các ký tự [a-z0-9]")]
        public string? Slug { get; set; }

        [Display(Name = "Mô tả ngắn")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
        public string? DesCriptions { get; set; }

        [Display(Name = "Nội dung ")]
        [DataType(DataType.Text)]
        public string? Contents { get; set; }

        [Display(Name = "Giá sản phẩm")]
        public decimal? Price { get; set; }

        [Display(Name = "Giảm giá còn")]
        public decimal? PriceSale { get; set; }

        [Display(Name = "Số lượng")]
        [Range(0, int.MaxValue)]
        public int? Quantity { get; set; }

        [Display(Name = "Màu sắc")]
        public string? Color { get; set; }

        [Display(Name = "Loại")]
        public string? TypeProduct { get; set; }

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime? DateCreate { get; set; }

        [Display(Name = "Ngày cập nhật")]
        [DataType(DataType.DateTime)]
        public DateTime? DateUpdate { get; set; }

        [Display(Name = "Hiển thị")]
        public bool IsActive { get; set; }
        [Display(Name = "Giảm giá")]
        public bool IsSale { get; set; }
        [Display(Name = "Hiển thị trang Home")]
        public bool IsHome { get; set; }
        public string? SeoTitle { get; set; }
        public string? SeoKeyword { get; set; }
        public string? SeoDesCriptions { get; set; }
        public List<ProductPhoto>? ProductPhoto { get; set; }
        public List<ProductCategory>? ProductCategory { get; set; }

        [Display(Name = "Đã bán")]
        [Range(0, int.MaxValue)]
        public int? Sold { get; set; }
    }
}
