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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
