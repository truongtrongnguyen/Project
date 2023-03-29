using BangHang.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Models.Services
{
    public class CartItemModel
    {
        private Products products { get; set; }
        private readonly AppDbContext _context;

        public int? quantity { get; set; }
        public Products? Product
        {
            get
            {
                return products;
            }
            set { products = value; }
        }

    }
}
