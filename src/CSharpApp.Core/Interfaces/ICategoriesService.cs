namespace CSharpApp.Core.Interfaces

{
    public interface ICategoriesService
    {
        Task<IReadOnlyCollection<Category>> GetCategories();
        Task<Category?> GetCategoryByID(int id);
        Task<Category?> CreateNewCategory(CreateCategoryRequest request);
    }
}