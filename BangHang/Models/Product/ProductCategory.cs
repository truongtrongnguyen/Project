namespace BangHang.Models.Product
{
    public class ProductCategory
    {
        public int? CategoryID { get; set; }
        public int? ProductID { get; set; }
        public CategoryPro? Category { get; set; }
        public Products? Product { get; set; }
    }
}
