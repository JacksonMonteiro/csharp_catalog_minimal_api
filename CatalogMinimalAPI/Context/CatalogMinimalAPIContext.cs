using CatalogMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogMinimalAPI.Context {
    public class CatalogMinimalAPIContext : DbContext {
        public CatalogMinimalAPIContext(DbContextOptions<CatalogMinimalAPIContext> options) : base(options) { }

        public DbSet<Product>? Produts { get; set; }
        public DbSet<Category>? Categories { get; set; }
    }
}
