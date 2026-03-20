using System.Net.Http.Json;

namespace CSharpApp.Application.Categories

{
    public class CategoriesService : ICategoriesService
    {
        private readonly HttpClient _httpClient;
        private readonly RestApiSettings _restApiSettings;
        private readonly ILogger<CategoriesService> _logger;

        public CategoriesService(HttpClient httpClient, IOptions<RestApiSettings> restApiSettings, ILogger<CategoriesService> logger)
        {
            _httpClient = httpClient;
            _restApiSettings = restApiSettings.Value;
            _logger = logger;
        }

        public async Task<IReadOnlyCollection<Category>> GetCategories()
        {
            var response = await _httpClient.GetAsync(_restApiSettings.Categories);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var categories = JsonSerializer.Deserialize<List<Category>>(content) ?? new List<Category>();

            return categories.AsReadOnly();
        }

        public async Task<Category?> GetCategoryByID(int id)
        {
            var response = await _httpClient.GetAsync($"{_restApiSettings.Categories}/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return await response.Content.ReadFromJsonAsync<Category>();
        }
    }
}