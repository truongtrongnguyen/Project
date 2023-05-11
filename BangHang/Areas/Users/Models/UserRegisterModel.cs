using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BangHang.Areas.Users.Models
{
    public class UserRegisterModel
    {
        [DisplayName("Email")]
        [Required(ErrorMessage = "Phải nhập Email")]
        [EmailAddress(ErrorMessage = "Không đúng định dạng Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phải nhập mật khẩu")]
        [DisplayName("Mật khẩu")]
        [StringLength(maximumLength: 20, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ {2} đến {1} ký tự, phải chứa một ký tự đặc biệt")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Lặp lại mật khẩu")]
        [DisplayName("Xác nhận mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu lặp lại không chính xác")]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Phải nhập tên người dùng")]
        [DisplayName("Họ tên người dùng")]
        [StringLength(maximumLength: 30, MinimumLength = 6, ErrorMessage = "{1} phải từ {2} đến {1} ký tự")]
        public string? FullName { get; set; }
    }
}
