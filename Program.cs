using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using utmMarker.Infrastructure.Data;
using utmMarker.Core.Repositories;
using utmMarker.Application; // Ensure this is present
using utmMarker.Core.UseCases; // Ensure this is present

// Main entry point
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Configure User Secrets for development
        builder.Configuration.AddUserSecrets<ConsoleUserInterface>(); // Use a non-static class from the assembly 

        // Add services to the container.
        // Product Use Cases
        builder.Services.AddScoped<IRetrieveAllProductsUseCase, RetrieveAllProductsUseCaseImpl>();
        builder.Services.AddScoped<IRetrieveProductByIdUseCase, RetrieveProductByIdUseCaseImpl>();
        builder.Services.AddScoped<IFilterProductsUseCase, FilterProductsUseCaseImpl>();
        builder.Services.AddScoped<ICreateProductUseCase, CreateProductUseCaseImpl>();
        builder.Services.AddScoped<IUpdateProductUseCase, UpdateProductUseCaseImpl>();
        builder.Services.AddScoped<IUpdateProductStockUseCase, UpdateProductStockUseCaseImpl>();
        builder.Services.AddScoped<ILowStockAlertUseCase, LowStockAlertUseCaseImpl>();

        // Sale Use Cases
        builder.Services.AddScoped<IFetchAllSalesUseCase, FetchAllSalesUseCaseImpl>();
        builder.Services.AddScoped<IFetchSaleByIdUseCase, FetchSaleByIdUseCaseImpl>();
        builder.Services.AddScoped<IFetchSalesByFilterUseCase, FetchSalesByFilterUseCaseImpl>();
        builder.Services.AddScoped<ICreateSaleUseCase, CreateSaleUseCaseImpl>();
        builder.Services.AddScoped<IUpdateSaleStatusUseCase, UpdateSaleStatusUseCaseImpl>();

        // Register the ConsoleUserInterface as the main application entry point
        builder.Services.AddScoped<ConsoleUserInterface>();

        builder.Services.AddInfrastructure(); // Call the extension method

        var host = builder.Build();

        // Retrieve the ConsoleUserInterface from the DI container and run it within a service scope
        await using (var scope = host.Services.CreateAsyncScope()) // Create a manual IServiceScope
        {
            var app = scope.ServiceProvider.GetRequiredService<ConsoleUserInterface>();
            await app.RunAsync();
        }
        
        // This host.RunAsync() will now keep the console application alive
        // without ConsoleApplication managing it, as ConsoleUserInterface
        // has its own loop. So it's better to remove it if ConsoleUserInterface
        // manages the application lifecycle.
        // await host.RunAsync(); 
    }
}

