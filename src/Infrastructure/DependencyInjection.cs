namespace Microsoft.Extensions.DependencyInjection;

using utmMarker.Infrastructure.Data;
using utmMarker.Core.Repositories;
using utmMarker.Infrastructure.Repositories;
using utmMarker.Core.UseCases; // Added for UseCases
using utmMarker.Application; // Added for UseCase Implementations

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddSingleton<IProductRepository, ProductRepositoryImpl>();
        services.AddScoped<ISaleRepository, SaleRepositoryImpl>();
        services.AddScoped<ICustomerRepository, CustomerRepositoryImpl>();

        // Register new customer-related use cases
        services.AddScoped<IEmailValidationUseCase, EmailValidationUseCaseImpl>();
        services.AddScoped<ICreateCustomerUseCase, CreateCustomerUseCaseImpl>();
        services.AddScoped<IRetrieveAllCustomersUseCase, RetrieveAllCustomersUseCaseImpl>();
        // Register other infrastructure services here
        return services;
    }
}
