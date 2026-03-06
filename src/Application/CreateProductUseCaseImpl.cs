using System;
using System.Threading;
using System.Threading.Tasks;
using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public sealed class CreateProductUseCaseImpl(IProductRepository productRepository) : ICreateProductUseCase
{
    public async Task<Product> ExecuteAsync(Product product, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(product, nameof(product));

        // For new products, ProductID should be 0. The repository will assign a generated ID.
        // If the incoming product already has an ID, it implies an existing product (though this use case is for creation).
        // For creation, we ensure the ID is 0, letting the DB handle generation.
        if (product.ProductID != 0)
        {
            // If the user somehow provided a non-zero ID for creation,
            // we might decide to throw an error or reset it to 0.
            // For now, let's create a new instance with ID 0.
            product = new Product(0, product.Name, product.SKU, product.Brand, product.Price, product.Stock); // Updated constructor call
        }
        
        // Further business validation can go here (e.g., check for unique SKU)

        return await productRepository.AddAsync(product, cancellationToken);
    }
}
