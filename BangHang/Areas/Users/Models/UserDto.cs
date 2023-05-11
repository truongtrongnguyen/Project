namespace BangHang.Areas.Users.Models
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Image { get; set; } = string.Empty;
        public IFormFile? Avata { get; set; }
        public DateTime BirthDay { get; set; }
        public string City { get; set; } = string.Empty;
        public string Distric { get; set; } = string.Empty;
        public string Wrad { get; set; } = string.Empty;
        public string AddressDetails { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public bool EmailConfirm { get; set; }
        public int OrderCount {get; set;}
    }
}
