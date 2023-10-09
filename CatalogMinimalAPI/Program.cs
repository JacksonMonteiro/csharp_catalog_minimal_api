using CatalogMinimalAPI.Context;
using CatalogMinimalAPI.Models;
using CatalogMinimalAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog Minimal API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = @"JWT Authorization header using the bearer scheme, Enter 'Bearer' [space].Example: \'Bearer 1234abcdef\'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<CatalogMinimalAPIContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// JWT Token
builder.Services.AddSingleton<ITokenService>(new TokenService());

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// API Endpoints
app.MapPost("/login", [AllowAnonymous] (User user, ITokenService tokenService) => {
    if (user == null) {
        return Results.BadRequest("Login Inválido");
    }

    if (user.Username == "jackson" && user.Password == "1234") {
        var tokenString = tokenService.GenerateToken(app.Configuration["Jwt:Key"],
            app.Configuration["Jwt:Issuer"],
            app.Configuration["Jwt:Audience"],
            user);

        return Results.Ok(new { token = tokenString });
    }
    else {
        return Results.BadRequest("Login Inválido");
    }
}).Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status200OK).WithName("Login").WithTags("Authentication");

// Categories
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

// Products
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
