using CatalogMinimalAPI.Context;
using CatalogMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogMinimalAPI.APIEndpoints {
    public static class CategoriesEndpoints {
        public static void MapCategoriesEndpoints(this WebApplication app) {
            app.MapPost("/categories", async (Category category, CatalogMinimalAPIContext db) => {
                db.Categories.Add(category);
                await db.SaveChangesAsync();
                return Results.Created($"/categories/{category.Id}", category);
            });

            app.MapGet("/categories", async (CatalogMinimalAPIContext db) => await db.Categories.ToListAsync()).RequireAuthorization();

            app.MapGet("/categories/{id:int}", async (int id, CatalogMinimalAPIContext db) => {
                return await db.Categories.FindAsync(id) is Category category ? Results.Ok(category) : Results.NotFound();

            });

            app.MapPut("/categories/{id:int}", async (int id, Category category, CatalogMinimalAPIContext db) => {
                if (category.Id != id) {
                    return Results.BadRequest();
                }

                var categoryDB = await db.Categories.FindAsync(id);
                if (categoryDB is null) return Results.NotFound();

                categoryDB.Name = category.Name;
                categoryDB.Description = category.Description;

                await db.SaveChangesAsync();
                return Results.Ok(categoryDB);
            });

            app.MapDelete("/categories/{id:int}", async (int id, CatalogMinimalAPIContext db) => {
                var category = await db.Categories.FindAsync(id);
                if (category is null) {
                    return Results.NotFound();
                }

                db.Categories.Remove(category);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
    }
}
