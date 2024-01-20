using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalAgency.Dal.Context;

/// <summary>
///     Migration service
/// </summary>
public class MigrationsService : IHostedService
{
    readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="serviceProvider"></param>
    public MigrationsService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Starts migration
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        await using ServicingContext productManagerContext = scope.ServiceProvider.GetRequiredService<ServicingContext>();
        await productManagerContext.Database.MigrateAsync(cancellationToken);
        await SeedingExtension.PopulateDatabase(productManagerContext);
    }

    /// <summary>
    ///     Stops migration
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}