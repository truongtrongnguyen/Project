using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BangHang.Areas.Users.Models
{
    public class UserLoginModel
    {
        [DisplayName("Email")]
        [Required(ErrorMessage = "Phải nhập Email")]
        [EmailAddress(ErrorMessage = "Không đúng định dạng Email")]
        public string Email { get; set; }


        [Required(ErrorMessage = "Phải nhập mật khẩu")]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
    }
}
