using CatalogMinimalAPI.APIEndpoints;
using CatalogMinimalAPI.AppServicesExtensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddApiSwagger();
builder.AddPersistence();
builder.Services.AddCors();
builder.AddAuthenticationJWT();

var app = builder.Build();
app.MapAuthenticationEndpoints();
app.MapCategoriesEndpoints();
app.MapProductsEndpoints();

var environment = app.Environment;
app.UseExceptionHandling(environment).UseSwaggerMiddleware().UseAppCors();
app.UseHttpsRedirection();

app.Run();