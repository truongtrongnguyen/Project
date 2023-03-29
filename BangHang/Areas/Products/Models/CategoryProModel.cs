using BangHang.Models.Product;
using System.ComponentModel.DataAnnotations;

namespace BangHang.Areas.Products.CategoryProModels
{
	public class CategoryProModel : CategoryPro
	{
		[Display(Name = "Ảnh đại diện")]
		public IFormFile? Avata { get; set; }
	}
}
