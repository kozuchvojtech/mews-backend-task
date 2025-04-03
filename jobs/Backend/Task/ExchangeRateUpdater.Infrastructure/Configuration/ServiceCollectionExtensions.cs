using ExchangeRateUpdater.Application.Common;
using ExchangeRateUpdater.Infrastructure.CnbApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;

namespace ExchangeRateUpdater.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddRefitClient<ICnbApi>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.cnb.cz"))
            .AddResilienceHandler("default", builder =>
            {
                builder.AddRetry(new HttpRetryStrategyOptions
                {
                    Delay = TimeSpan.FromSeconds(5),
                    MaxRetryAttempts = 3,
                    BackoffType = DelayBackoffType.Linear,
                    UseJitter = true
                })
                .AddTimeout(TimeSpan.FromSeconds(30));
            });

        services.AddScoped<IExchangeRateProvider, CnbExchangeRateProvider>();

        return services;
    }
}