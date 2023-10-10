using CatalogMinimalAPI.Context;
using CatalogMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogMinimalAPI.APIEndpoints {
    public static class ProductsEndpoints {
        public static void MapProductsEndpoints(this WebApplication app) {
            app.MapPost("/products", async (Product product, CatalogMinimalAPIContext db) => {
                db.Produts.Add(product);
                await db.SaveChangesAsync();
                return Results.Created($"/categories/{product.Id}", product);
            });

            app.MapGet("/products", async (CatalogMinimalAPIContext db) => await db.Produts.ToListAsync()).RequireAuthorization();

            app.MapGet("/products/{id:int}", async (int id, CatalogMinimalAPIContext db) => {
                return await db.Produts.FindAsync(id) is Product product ? Results.Ok(product) : Results.NotFound();

            });

            app.MapPut("/products/{id:int}", async (int id, Product product, CatalogMinimalAPIContext db) => {
                if (product.Id != id) {
                    return Results.BadRequest();
                }

                var productDB = await db.Produts.FindAsync(id);
                if (productDB is null) return Results.NotFound();

                productDB.Name = product.Name;
                productDB.Description = product.Description;
                productDB.Price = product.Price;
                productDB.ImagePath = product.ImagePath;
                productDB.PurchaseDate = product.PurchaseDate;
                productDB.Stock = product.Stock;
                productDB.CategoryId = product.CategoryId;


                await db.SaveChangesAsync();
                return Results.Ok(productDB);
            });

            app.MapDelete("/products/{id:int}", async (int id, CatalogMinimalAPIContext db) => {
                var product = await db.Produts.FindAsync(id);
                if (product is null) {
                    return Results.NotFound();
                }

                db.Produts.Remove(product);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
