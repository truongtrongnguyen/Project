using System.ComponentModel.DataAnnotations;

namespace BangHang.Models.OderProduct
{
    public class Order
    {
        [Key] 
        public int? Id { get; set; }

        [Display(Name = "Mã đơn hàng")]
        public string? Code { get; set; }

        [Display(Name = "Tên khách hàng")]
        [Required(ErrorMessage = "Phải nhập {0}")]
        public string CustomerName { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Phải nhập {0}")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Display(Name = "Địa chỉ")]
        [Required(ErrorMessage = "Phải nhập {0}")]
        public string Address { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal? TotalAmount { get; set; }

        [Display(Name = "Số lượng")]
        public int? Quantity { get; set; }

        [Display(Name = "Loại thanh toán")]
        public int? Payment { get; set; }

        [Display(Name = "Ngày tạo")]
        [DataType(DataType.DateTime)]
        public DateTime? DateCreate { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Required(ErrorMessage = "Phải nhập {0}")]
        public string Email { get; set; }

        public string? UserId { get; set; }

        public List<OrderDetail>? OrderDetails { get; set; }
    }
}
