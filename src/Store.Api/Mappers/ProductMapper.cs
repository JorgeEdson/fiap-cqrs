using Store.Api.Dtos;
using Store.Domain;

namespace Store.Api.Mappers;

public static class ProductMapper
{
    public static Product ToProduct(this CreateProductDto request)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
            Discontinued = false
        };
    }

    public static Product ToProduct(this UpdateProductDto request, Guid id)
    {
        return new Product
        {
            Id = id,
            Name = request.Name,
            Price = request.Price,
            Stock = request.Stock,
            Discontinued = request.Discontinued
        };
    }

    public static ProductDto ToProductDto(this Product product)
    {
        return new ProductDto(product.Id, product.Name, product.Price, product.Stock, product.Discontinued);
    }

    public static IEnumerable<ProductDto> ToProductDtos(this IEnumerable<Product> products)
    {
        return products.Select(p => p.ToProductDto());
    }

    public static void UpdateProductFromDto(this Product product, UpdateProductDto request)
    {
        product.Name = request.Name;
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.Discontinued = request.Discontinued;
    }
}
