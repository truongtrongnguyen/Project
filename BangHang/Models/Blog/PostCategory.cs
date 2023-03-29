namespace BangHang.Models.Blog
{
    public class PostCategory
    {
        public int? CategoryID { get; set; }
        public int? PostID { get; set; }
        public Category? Category { get; set; }
        public Post? Post { get; set; }
    }
}
