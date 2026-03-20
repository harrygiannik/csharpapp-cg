using Polly;
using Polly.Extensions.Http;

namespace CSharpApp.Infrastructure.Configuration;

public static class HttpConfiguration
{
    public static IServiceCollection AddHttpConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var clientSettings = configuration.GetSection(nameof(HttpClientSettings)).Get<HttpClientSettings>() ?? new HttpClientSettings();
        var apiSettings = configuration.GetSection(nameof(RestApiSettings)).Get<RestApiSettings>() ?? new RestApiSettings();

        var handlerLifetime = clientSettings.LifeTime > 0
            ? TimeSpan.FromMinutes(clientSettings.LifeTime)
            : TimeSpan.FromMinutes(2);

        services
            .AddHttpClient<IProductsService, ProductsService>(client =>
            {
                if (!string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
                {
                    client.BaseAddress = new Uri(apiSettings.BaseUrl);
                }
            })
            .SetHandlerLifetime(handlerLifetime)
            .AddPolicyHandler(GetRetryPolicy(clientSettings));

        services
            .AddHttpClient<ICategoriesService, CategoriesService>(client =>
            {
                if (!string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
                {
                    client.BaseAddress = new Uri(apiSettings.BaseUrl);
                }
            })
            .SetHandlerLifetime(handlerLifetime)
            .AddPolicyHandler(GetRetryPolicy(clientSettings));

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(HttpClientSettings settings)
    {
        const int defaultRetryCount = 2;
        const int defaultSleepMs = 200;

        var retryCount = settings.RetryCount > 0 ? settings.RetryCount : defaultRetryCount;
        var sleepDurationMs = settings.SleepDuration > 0 ? settings.SleepDuration : defaultSleepMs;

        return HttpPolicyExtensions.HandleTransientHttpError()
            .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromMilliseconds(sleepDurationMs));
    }
}
