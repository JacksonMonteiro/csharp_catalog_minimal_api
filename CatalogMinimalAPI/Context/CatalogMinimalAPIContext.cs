using CatalogMinimalAPI.Models;
 using Microsoft.EntityFrameworkCore;

namespace CatalogMinimalAPI.Context {
    public class CatalogMinimalAPIContext : DbContext {
        public CatalogMinimalAPIContext(DbContextOptions<CatalogMinimalAPIContext> options) : base(options) { }

        public DbSet<Product>? Produts { get; set; }
        public DbSet<Category>? Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Category>().Property(c => c.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Category>().Property(c => c.Description).HasMaxLength(150).IsRequired();

            modelBuilder.Entity<Product>().HasKey(p => p.Id);
            modelBuilder.Entity<Product>().Property(p => p.Name).HasMaxLength(100).IsRequired();
            modelBuilder.Entity<Product>().Property(p => p.Description).HasMaxLength(150);
            modelBuilder.Entity<Product>().Property(p => p.ImagePath).HasMaxLength(100);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(14, 2);

            modelBuilder.Entity<Product>()
                .HasOne<Category>(c => c.Category)
                    .WithMany(p => p.Products)
                        .HasForeignKey(c => c.CategoryId);
        }
    }
}
