using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.DataIO;

internal static class Extensions
{
    internal static IServiceCollection ConfigureDataImportExport(this IServiceCollection services)
    {
        services.AddTransient<IDataExport, DataExport>();
        services.AddTransient<IDataImport, DataImport>();

        return services;
    }
}
