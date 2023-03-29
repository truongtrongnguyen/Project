using Microsoft.EntityFrameworkCore;

namespace BangHang.Models.Product
{
    public class CartItem
    {
        public int? quantity { get; set; }
        public int? ProductId { get; set; }

    }
}
