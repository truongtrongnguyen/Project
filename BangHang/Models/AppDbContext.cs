using BangHang.Models.Blog;
using BangHang.Models.OderProduct;
using BangHang.Models.Product;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BangHang.Models
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :base(options)
        { 
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            base.OnConfiguring(optionBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Blog
            builder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(x => new { x.PostID, x.CategoryID });
            });
            builder.Entity<Category>(entity =>
            {
                entity.HasIndex(x => x.Slug).IsUnique();
            });
            builder.Entity<Post>(entity =>
            {
                entity.HasIndex(x => x.Slug).IsUnique();
            });

            // Product
            builder.Entity<ProductCategory>(entity =>
            {
                entity.HasKey(x => new { x.ProductID, x.CategoryID });
            });
            builder.Entity<CategoryPro>(entity =>
            {
                entity.HasIndex(x => x.Slug).IsUnique();
            });
            builder.Entity<Products>(entity =>
            {
                entity.HasIndex(x => x.Slug).IsUnique();
            });

            // Order
            builder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(x => new { x.ProductID, x.OrderID });
            });
            builder.Entity<Order>(entity =>
            {
                entity.HasIndex(x => x.Code).IsUnique();
            });
        }
        // Blog
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategory { get; set; }

        // Product
        public DbSet<CategoryPro> CategoriesPro { get; set;}
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }

        // Order
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
