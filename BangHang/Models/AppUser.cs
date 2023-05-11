using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BangHang.Models
{
    public class AppUser : IdentityUser
    {
        [Required(ErrorMessage = "Phải nhập tên người dùng")]
        public string FullName { get; set; }
        public DateTime? BirthDay { get; set; }
        public string? Avata { get; set; }
        public string? Sex { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? DetailsAddress { get; set; }
        public int? OrdersCount { get; set; }

    }
}
