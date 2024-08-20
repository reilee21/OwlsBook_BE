using Microsoft.Extensions.DependencyInjection;

namespace Crawler
{
    public static class RegisterCrawlServices
    {
        public static IServiceCollection AddCrawlService(this IServiceCollection services)
        {
            services.AddScoped<ScraperServices>();
            return services;
        }
    }
}
