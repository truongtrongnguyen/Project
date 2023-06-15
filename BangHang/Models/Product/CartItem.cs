using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace BangHang.Models.Product
{
    public class CartItem
    {
        public int? quantity { get; set; }
        public int? ProductId { get; set; }
        public decimal? Price { get; set; }
        public decimal? PriceSale { get; set; }
        public string? NameProduct { get; set; }
        public string? ImageAvata { get; set; }
        public string? Color { get; set; }
        public string? TypeProduct { get; set; }
    }
}
