using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using utmMarker.Core.Entities;
using utmMarker.Core.UseCases;
using utmMarker.Core.Filters; // Added for SaleFilter
using Microsoft.Extensions.DependencyInjection; // Added for IServiceScopeFactory
using System.Collections.Generic; // Added for List<SaleDetail>

namespace utmMarker.Application;

public sealed class ConsoleUserInterface(
    ILogger<ConsoleUserInterface> logger,
    IServiceScopeFactory serviceScopeFactory, // Inject IServiceScopeFactory
    IRetrieveAllProductsUseCase retrieveAllProductsUseCase,
    IRetrieveProductByIdUseCase retrieveProductByIdUseCase,
    ICreateProductUseCase createProductUseCase,
    IUpdateProductStockUseCase updateProductStockUseCase, // Injected for stock management
    ICreateSaleUseCase createSaleUseCase) // Injected for creating sales
{
    private readonly ILogger<ConsoleUserInterface> _logger = logger;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory; // Store IServiceScopeFactory
    private readonly IRetrieveAllProductsUseCase _retrieveAllProductsUseCase = retrieveAllProductsUseCase;
    private readonly IRetrieveProductByIdUseCase _retrieveProductByIdUseCase = retrieveProductByIdUseCase;
    private readonly ICreateProductUseCase _createProductUseCase = createProductUseCase;
    private readonly IUpdateProductStockUseCase _updateProductStockUseCase = updateProductStockUseCase; // Stored for use
    private readonly ICreateSaleUseCase _createSaleUseCase = createSaleUseCase; // Stored for use

    private static readonly string _menuArt = "=========================================== || UTM Marker Product & Customer Management || =========================================== || 1. List All Products || 2. Get Product by ID || 3. Create New Product || 4. Register Sale (Point of Sale) || 5. Consultar venta por fechas || 6. Register New Customer || 7. View All Customers || 8. Send Email to Customers || 9. Exit || =========================================== Please enter your choice: ";

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando la interfaz de usuario de consola...");

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(_menuArt);
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ListAllProductsAsync(cancellationToken);
                    break;
                case "2":
                    await GetProductByIdAsync(cancellationToken);
                    break;
                case "3":
                    await CreateNewProductAsync(cancellationToken);
                    break;
                case "4":
                    await RegisterSaleAsync(cancellationToken); // New case for point of sale
                    break;
                case "5":
                    await ConsultarVentaPorFechasAsync(cancellationToken);
                    break;
                case "6":
                    await RegisterNewCustomerAsync(cancellationToken);
                    break;
                case "7":
                    await ViewAllCustomersAsync(cancellationToken);
                    break;
                case "8":
                    await SendEmailToCustomersAsync(cancellationToken);
                    break;
                case "9":
                    _logger.LogInformation("Exiting application.");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private async Task RegisterSaleAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\n--- Registrar Venta (Punto de Venta) ---");

        var saleDetails = new List<SaleDetail>();
        decimal totalSaleAmount = 0;
        int totalArticles = 0;

        while (true)
        {
            Console.Write("Ingrese el ID del producto (o 'fin' para terminar): ");
            string? productIdInput = Console.ReadLine();

            if (productIdInput?.ToLower() == "fin")
            {
                break;
            }

            if (!int.TryParse(productIdInput, out int productId))
            {
                Console.WriteLine("ID de producto inválido. Intente de nuevo.");
                continue;
            }

            Console.Write($"Ingrese la cantidad para el producto {productId}: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Cantidad inválida. Debe ser un número entero positivo. Intente de nuevo.");
                continue;
            }

            try
            {
                // Retrieve product details to validate and get price/stock
                var product = await _retrieveProductByIdUseCase.ExecuteAsync(productId, cancellationToken);

                if (product == null)
                {
                    Console.WriteLine($"Producto con ID {productId} no encontrado.");
                    continue;
                }

                if (product.Stock < quantity)
                {
                    Console.WriteLine($"Stock insuficiente para el producto {product.Name}. Stock disponible: {product.Stock}");
                    continue;
                }

                // Add to sale details
                decimal detailTotal = product.Price * quantity;
                saleDetails.Add(new SaleDetail(
                    0, // DetalleID will be generated by the database
                    0, // VentaID will be set after sale creation
                    productId,
                    product.Price,
                    quantity,
                    detailTotal
                ));

                totalSaleAmount += detailTotal;
                totalArticles += quantity;
                Console.WriteLine($"Producto '{product.Name}' (x{quantity}) agregado a la venta.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al agregar producto {productId} a la venta.");
                Console.WriteLine("Ocurrió un error al procesar el producto. Intente de nuevo.");
            }
        }

        if (!saleDetails.Any())
        {
            Console.WriteLine("No se agregaron productos a la venta. Operación cancelada.");
            return;
        }

        Console.WriteLine($"\nResumen de la Venta:");
        Console.WriteLine($"Total Artículos: {totalArticles}");
        Console.WriteLine($"Monto Total: {totalSaleAmount:C}");
        Console.Write("Confirmar venta (s/n)? ");
        if (Console.ReadLine()?.ToLower() != "s")
        {
            Console.WriteLine("Venta cancelada.");
            return;
        }

        try
        {
            // Generate a unique folio for the sale
            string folio = $"VENTA-{DateTime.Now:yyMMddHHmm}-{Guid.NewGuid().ToString().Substring(0, 2).ToUpper()}";

            // Create the Sale object
            var newSale = new Sale(
                0, // VentaID will be generated by the database
                folio,
                DateTime.Now,
                totalArticles,
                totalSaleAmount,
                Core.Enums.SaleStatus.Completed // Assuming a completed status for console sales
            );

            // Create the sale in the database, also inserting sale details
            var createdSale = await _createSaleUseCase.ExecuteAsync(newSale, saleDetails, cancellationToken);
            Console.WriteLine($"Venta con Folio '{createdSale.Folio}' creada exitosamente. ID: {createdSale.SaleID}");

            // Update product stock for each sold item
            foreach (var detail in saleDetails)
            {
                await _updateProductStockUseCase.ExecuteAsync(detail.ProductoID, -detail.Quantity, cancellationToken);
                _logger.LogInformation($"Stock actualizado para ProductoID {detail.ProductoID}: -{detail.Quantity}");
            }
            Console.WriteLine("Stocks de productos actualizados.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar la venta.");
            Console.WriteLine("Ocurrió un error crítico al registrar la venta. Los cambios pueden no haberse guardado.");
        }
    }

    private async Task ListAllProductsAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\n--- All Products ---");
        try
        {
            var products = _retrieveAllProductsUseCase.ExecuteAsync(cancellationToken);
            await foreach (var product in products.WithCancellation(cancellationToken))
            {
                Console.WriteLine($"ID: {product.ProductID}");
                Console.WriteLine($"Name: {product.Name}");
                Console.WriteLine($"SKU: {product.SKU}");
                Console.WriteLine($"Brand: {product.Brand}");
                Console.WriteLine($"Price: {product.Price:C}");
                Console.WriteLine($"Stock: {product.Stock}");
                Console.WriteLine("--------------------");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing all products.");
            Console.WriteLine("An error occurred while fetching products.");
        }
    }

    private async Task GetProductByIdAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\n--- Get Product by ID ---");
        Console.Write("Enter Product ID (int): "); // Changed prompt
        string? idInput = Console.ReadLine();

        if (int.TryParse(idInput, out int productId)) // Changed to int.TryParse and int productId
        {
            try
            {
                var product = await _retrieveProductByIdUseCase.ExecuteAsync(productId, cancellationToken);
                if (product != null)
                {
                    Console.WriteLine($"ID: {product.ProductID}");
                    Console.WriteLine($"Name: {product.Name}");
                    Console.WriteLine($"SKU: {product.SKU}");
                    Console.WriteLine($"Brand: {product.Brand}");
                    Console.WriteLine($"Price: {product.Price:C}");
                    Console.WriteLine($"Stock: {product.Stock}");
                }
                else
                {
                    Console.WriteLine($"Product with ID {productId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting product by ID {productId}.");
                Console.WriteLine("An error occurred while fetching the product.");
            }
        }
        else
        {
            Console.WriteLine("Invalid integer format for Product ID."); // Updated error message
        }
    }

    private async Task CreateNewProductAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\n--- Create New Product ---");
        
        Console.Write("Enter Product Name: ");
        string name = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name cannot be empty."); return; }

        Console.Write("Enter Product SKU: ");
        string sku = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(sku)) { Console.WriteLine("SKU cannot be empty."); return; }

        Console.Write("Enter Product Brand: ");
        string brand = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(brand)) { Console.WriteLine("Brand cannot be empty."); return; }

        decimal price;
        Console.Write("Enter Product Price: ");
        if (!decimal.TryParse(Console.ReadLine(), out price) || price < 0) { Console.WriteLine("Invalid Price. Must be a non-negative number."); return; }

        int stock;
        Console.Write("Enter Product Stock: ");
        if (!int.TryParse(Console.ReadLine(), out stock) || stock < 0) { Console.WriteLine("Invalid Stock. Must be a non-negative integer."); return; }

        try
        {
            var newProduct = new Product(0, name, sku, brand, price, stock); // Updated constructor call
            var createdProduct = await _createProductUseCase.ExecuteAsync(newProduct, cancellationToken);
            Console.WriteLine($"Product '{createdProduct.Name}' created successfully with ID: {createdProduct.ProductID}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new product.");
            Console.WriteLine("An error occurred while creating the product.");
        }
    }

    private async Task ConsultarVentaPorFechasAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\n--- Consultar Venta por Fechas ---");

        DateTime startDate = GetDateInput("Ingrese la fecha de inicio (yyyy-MM-dd):");
        DateTime endDate = GetDateInput("Ingrese la fecha de fin (yyyy-MM-dd):");

        if (startDate > endDate)
        {
            Console.WriteLine("La fecha de inicio no puede ser posterior a la fecha de fin. Inténtelo de nuevo.");
            return;
        }

        var filter = new SaleFilter
        {
            MinSaleDate = startDate,
            MaxSaleDate = endDate.AddDays(1).AddTicks(-1) // End of day for MaxSaleDate
        };

        // Create a new scope for the use case
        await using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var fetchSalesByFilterUseCase = scope.ServiceProvider.GetRequiredService<IFetchSalesByFilterUseCase>();

            try
            {
                var sales = fetchSalesByFilterUseCase.ExecuteAsync(filter, cancellationToken);

                Console.WriteLine("\n-----------------------------------------------------------");
                Console.WriteLine($"| {"Folio",-10} | {"Fecha",-20} | {"Monto Total",-15} |");
                Console.WriteLine("-----------------------------------------------------------");

                bool foundSales = false;
                await foreach (var sale in sales.WithCancellation(cancellationToken))
                {
                    foundSales = true;
                    Console.WriteLine($"| {sale.Folio,-10} | {sale.SaleDate,-20:yyyy-MM-dd HH:mm} | {sale.TotalSale,-15:C} |");
                }

                if (!foundSales)
                {
                    Console.WriteLine("| No se encontraron ventas para el rango de fechas especificado.       |");
                }
                Console.WriteLine("-----------------------------------------------------------");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consultando ventas por fechas.");
                Console.WriteLine("Ocurrió un error al consultar las ventas por fechas.");
            }
        }
    }

    // New methods for Customer Management
    private async Task RegisterNewCustomerAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\n--- Register New Customer ---");

        Console.Write("Enter Customer Full Name: ");
        string fullName = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(fullName)) { Console.WriteLine("Full Name cannot be empty."); return; }

        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(email)) { Console.WriteLine("Email cannot be empty."); return; }

        await using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var createCustomerUseCase = scope.ServiceProvider.GetRequiredService<ICreateCustomerUseCase>();
            try
            {
                await createCustomerUseCase.ExecuteAsync(fullName, email);
                Console.WriteLine($"Customer '{fullName}' registered successfully.");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering new customer.");
                Console.WriteLine("An error occurred while registering the customer.");
            }
        }
    }

    private async Task ViewAllCustomersAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\n--- All Customers ---");
        await using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var retrieveAllCustomersUseCase = scope.ServiceProvider.GetRequiredService<IRetrieveAllCustomersUseCase>();
            try
            {
                var customers = await retrieveAllCustomersUseCase.ExecuteAsync();

                if (!customers.Any())
                {
                    Console.WriteLine("No customers registered yet.");
                    return;
                }

                Console.WriteLine("\n-----------------------------------------------------------");
                Console.WriteLine($"| {"ID",-4} | {"Full Name",-25} | {"Email",-30} | {"Active",-6} |");
                Console.WriteLine("-----------------------------------------------------------");

                foreach (var customer in customers)
                {
                    Console.WriteLine($"| {customer.Id,-4} | {customer.FullName,-25} | {customer.Email,-30} | {customer.IsActive,-6} |");
                }
                Console.WriteLine("-----------------------------------------------------------");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing all customers.");
                Console.WriteLine("An error occurred while fetching customers.");
            }
        }
    }

    private async Task SendEmailToCustomersAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("\n--- Send Email to Customers ---");
        await using (var scope = _serviceScopeFactory.CreateAsyncScope())
        {
            var retrieveAllCustomersUseCase = scope.ServiceProvider.GetRequiredService<IRetrieveAllCustomersUseCase>();
            try
            {
                var customers = await retrieveAllCustomersUseCase.ExecuteAsync();

                if (!customers.Any())
                {
                    Console.WriteLine("No customers registered yet to send emails to.");
                    return;
                }

                Console.WriteLine("\n--- Customer Emails ---");
                foreach (var customer in customers)
                {
                    Console.WriteLine($"- {customer.Email}");
                }
                Console.WriteLine("\nNote: This is a placeholder. In a real application, you would integrate an email sending service here.");
                Console.WriteLine("You can use this list of emails to send out newsletters or promotional content manually.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing to send emails to customers.");
                Console.WriteLine("An error occurred while fetching customer emails.");
            }
        }
    }

    private DateTime GetDateInput(string prompt)
    {
        DateTime date;
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();
            if (DateTime.TryParse(input, out date))
            {
                return date;
            }
            Console.WriteLine("Formato de fecha inválido. Por favor, use el formato yyyy-MM-dd.");
        }
    }
}
