using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BangHang.Models.Product
{
    public class ProductPhoto
    {
        [Key]
        public int? Id { get; set; }

        public string? Name { get; set; }
        public string? UrlName { get; set; }
        public int? ProductID { get; set; }
        [ForeignKey("ProductID")]
        public Products? Product { get; set; }
    }
}
