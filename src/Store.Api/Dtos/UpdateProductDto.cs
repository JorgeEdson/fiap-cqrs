namespace Store.Api.Dtos;

public record UpdateProductDto(string Name, decimal Price, int Stock, bool Discontinued);
