namespace utmMarker.Core.Entities;

public sealed class Product(int productId, string name, string sku, string brand, decimal price, int stock)
{
    public int ProductID { get; init; } = productId;
    public string Name { get; init; } = name;
    public string SKU { get; init; } = sku;
    public string Brand { get; init; } = brand;

    private decimal _price = price; // Initialized from constructor parameter
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Price), "El precio no puede ser negativo.");
            }
            _price = value;
        }
    }

    private int _stock = stock; // Initialized from constructor parameter
    public int Stock
    {
        get => _stock;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Stock), "El stock no puede ser negativo.");
            }
            _stock = value;
        }
    }
}
