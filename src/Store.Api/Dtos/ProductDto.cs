namespace Store.Api.Dtos;

public record ProductDto(Guid Id, string Name, decimal Price, int Stock, bool Discontinued);
