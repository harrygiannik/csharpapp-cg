
namespace CSharpApp.Core.Dtos;

public record CreateProductRequest(
   [property: JsonPropertyName("title")] string Title,
   [property: JsonPropertyName("price")] int Price,
   [property: JsonPropertyName("description")] string Description,
   [property: JsonPropertyName("categoryId")] int CategoryId,
   [property: JsonPropertyName("images")] List<string> Images
);