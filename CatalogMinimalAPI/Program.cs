using CatalogMinimalAPI.Context;
using CatalogMinimalAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<CatalogMinimalAPIContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();

// API Endpoints
app.MapGet("/", () => "Catalog Minimal API - 2022");

// Categories
app.MapPost("/categories", async (Category category, CatalogMinimalAPIContext db) => {
    db.Categories.Add(category);
    await db.SaveChangesAsync();
    return Results.Created($"/categories/{category.Id}", category);
});

app.MapGet("/categories", async (CatalogMinimalAPIContext db) => await db.Categories.ToListAsync());

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

// Products
app.MapPost("/products", async (Product product, CatalogMinimalAPIContext db) => {
    db.Produts.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/categories/{product.Id}", product);
});

app.MapGet("/products", async (CatalogMinimalAPIContext db) => await db.Produts.ToListAsync());

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
    if (product  is null) {
        return Results.NotFound();
    }

    db.Produts.Remove(product);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
