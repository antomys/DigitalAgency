using DigitalAgency.Dal.Storages;
using DigitalAgency.Dal.Storages.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DigitalAgency.Api.Extensions
{
    public static class AddStoragesExtension
    {
        public static IServiceCollection AddStorages(this IServiceCollection services)
        {
            return services
                .AddTransient<IClientStorage, ClientStorage>()
                .AddTransient<IExecutorStorage, ExecutorStorage>()
                .AddTransient<IProjectStorage, ProjectStorage>()
                .AddTransient<IOrderStorage, OrderStorage>();
        }
    }
}