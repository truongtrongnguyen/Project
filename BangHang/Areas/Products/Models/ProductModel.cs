using BangHang.Models.Product;
using System.ComponentModel.DataAnnotations;

namespace BangHang.Product.Model
{
    public class ProductModel : Products
    {
        [Display(Name = "Danh mục")]
        public List<int>? CategoryIDs { get; set; }
        [Display(Name = "Ảnh sản phẩm")]
        public List<IFormFile>? ProductFiles { get; set; }
    }
}
