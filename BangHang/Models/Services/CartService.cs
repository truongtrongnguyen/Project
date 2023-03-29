
using BangHang.Models.Services;
using BangHang.Models.Product;
using Newtonsoft.Json;
namespace BangHang.Models.Services
{
    public class CartService
    {
        //Key lưu chuỗi json của cart
        public const string CARTKEY = "cart";

        private readonly IHttpContextAccessor _context;
        private readonly HttpContext HttpContext;
        public CartService(IHttpContextAccessor context)
        {
            _context = context;
            HttpContext = context.HttpContext;
        }

        // Lấy Cart từ Session (Danh sách CartItem)
        public List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session;
            string jsonCart = session.GetString(CARTKEY);
            if (jsonCart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsonCart);
            }
            return new List<CartItem>();
        }

        // Xóa cart khỏi Session
        public void CLearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào Session
        public void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsonCart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsonCart);
        }
    }
}