using BangHang.Models.Product;
using System.ComponentModel.DataAnnotations;

namespace BangHang.Models.OderProduct
{
    public class OrderDetail
    {
        [Key]
        public int? Id { get; set; }
        public int? OrderID { get; set; }

        public int? ProductID { get; set; }

        [Display(Name = "Giá sản phẩm")]
        public decimal? Price { get; set; }

        [Display(Name = "Giảm giá còn")]
        public decimal? PriceSale { get; set; }

        [Display(Name = "Số lượng")]
        public int? Quantity { get; set; }

        [Display(Name = "Tên sản phẩm")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
        public string? NameProduct { get; set; }

        [Display(Name = "Ảnh sản phẩm")]
        public string? ImageAvata { get; set; }

        [Display(Name = "Màu sắc")]
        public string? Color { get; set; }

        [Display(Name = "Loại")]
        public string? TypeProduct { get; set; }

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime? CreateDate { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Products? Product { get; set; }
    }
}
