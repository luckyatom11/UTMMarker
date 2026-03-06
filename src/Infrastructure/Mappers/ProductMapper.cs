namespace utmMarker.Infrastructure.Mappers;

using utmMarker.Core.Entities;
using utmMarker.Infrastructure.Models.Data;

public static class ProductMapper
{
    public static Product ToDomain(this ProductoEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        Product product = new Product(
            productId: entity.ProductoID, // Direct mapping, as both are int now
            name: entity.Nombre,
            sku: entity.SKU,
            brand: entity.Marca,
            price: entity.Precio, // New parameter
            stock: entity.Stock  // New parameter
        );

        return product;
    }

    public static ProductoEntity ToEntity(this Product domain)
    {
        ArgumentNullException.ThrowIfNull(domain);

        ProductoEntity entity = new ProductoEntity(
            productoId: domain.ProductID, // Direct mapping, as both are int now
            sku: domain.SKU
        )
        {
            Nombre = domain.Name,
            Marca = domain.Brand,
            Precio = domain.Price,
            Stock = domain.Stock
        };

        return entity;
    }
}
