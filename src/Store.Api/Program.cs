using Microsoft.EntityFrameworkCore;
using Store.Api.Dtos;
using Store.Api.Mappers;
using Store.Domain.Services;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Product CRUD Endpoints
var productsGroup = app.MapGroup("/api/products").WithTags("Products");


// GET: Get all products
productsGroup.MapGet("/", async (IProductService productService) =>
{
    var products = await productService.GetAllAsync();
    return Results.Ok(products.ToProductDtos());
})
.WithName("GetAllProducts");


// GET: Get product by ID
productsGroup.MapGet("/{id:guid}", async (Guid id, IProductService productService) =>
{
    var product = await productService.GetByIdAsync(id);
    return product is not null ? Results.Ok(product.ToProductDto()) : Results.NotFound();
})
.WithName("GetProductById");


// POST: Create a new product
productsGroup.MapPost("/", async (CreateProductDto request, IProductService productService) =>
{
    var product = await productService.CreateAsync(request.ToProduct());
    return Results.Created($"/api/products/{product.Id}", product.ToProductDto());
})
.WithName("CreateProduct");


// PUT: Update an existing product
productsGroup.MapPut("/{id:guid}", async (Guid id, UpdateProductDto request, IProductService productService) =>
{
    var updated = await productService.UpdateAsync(request.ToProduct(id));
    return updated ? Results.NoContent() : Results.NotFound();
})
.WithName("UpdateProduct");


// DELETE: Delete a product
productsGroup.MapDelete("/{id:guid}", async (Guid id, IProductService productService) =>
{
    var deleted = await productService.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteProduct");

app.Run();

