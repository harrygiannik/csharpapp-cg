namespace CSharpApp.Core.Interfaces;

public interface IProductsService
{
    Task<IReadOnlyCollection<Product>> GetProducts();
    Task<Product?> GetProductByID(int id);
    Task<Product?> CreateNewProduct(CreateProductRequest request);
}