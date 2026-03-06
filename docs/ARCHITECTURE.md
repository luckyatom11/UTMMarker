# Manifiesto Técnico de Arquitectura .NET 10 CLI

## 1. Resumen de Instalación de Dependencias NuGet

Se han instalado los siguientes paquetes NuGet, optimizados para .NET 10 y Native AOT, garantizando un alto rendimiento y un ciclo de vida de aplicación eficiente.

| Paquete NuGet | Versión | Rol Arquitectónico |
| :----------------------------------------- | :------ | :------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `Microsoft.Data.SqlClient` | 6.1.4 | Driver oficial para SQL Server, optimizado para Native AOT, facilitando el acceso a datos de alto rendimiento sin reflexión en tiempo de ejecución. |
| `Dapper` | 2.1.66 | Micro-ORM ligero (opcional, se debe usar con cautela en AOT para evitar reflexión dinámica, priorizando ADO.NET puro si es posible para asegurar compatibilidad total). |
| `Microsoft.Extensions.Hosting` | 10.0.3 | Proporciona funcionalidades clave para la gestión del ciclo de vida de la aplicación, inyección de dependencias (DI) y configuración, esencial para aplicaciones CLI robustas. |
| `Microsoft.Extensions.Configuration.UserSecrets` | 10.0.3 | Facilita la gestión segura de secretos de desarrollo local, evitando la exposición de información sensible en el control de versiones. |

## 2. Referencia de Implementación: `Program.cs`

A continuación, se presenta un esqueleto funcional de `Program.cs` que demuestra la configuración del host, la inyección de dependencias, un servicio de ejemplo y el uso de la sintaxis de C# 14, manteniendo la compatibilidad con Native AOT.

```csharp
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// Define a simple service to demonstrate DI and functionality
public interface IMyService
{
    ValueTask DoWorkAsync(CancellationToken cancellationToken = default);
}

public sealed class MyService : IMyService
{
    private readonly ILogger<MyService> _logger;
    private readonly string _connectionString;

    public MyService(ILogger<MyService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new InvalidOperationException("DefaultConnection not found.");
    }

    public async ValueTask DoWorkAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("MyService está realizando su trabajo...");
        
        // Simulate database operation (AOT-friendly ADO.NET)
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        // Example: C# 14 'field' keyword for a property (demonstration purposes)
        var dataProcessor = new DataProcessor { MaxRetries = 3 };
        _logger.LogInformation($"DataProcessor MaxRetries: {dataProcessor.MaxRetries}");

        _logger.LogInformation("Conexión a la base de datos simulada y operación completada.");

        await Task.Delay(100, cancellationToken); // Simulate async work
        _logger.LogInformation("MyService ha terminado su trabajo.");
    }
}

// Example class demonstrating C# 14 'field' keyword (AOT-friendly)
public sealed class DataProcessor
{
    // C# 14 'field' keyword usage
    public int MaxRetries { get => field; init => field = value; }
}

public sealed class ConsoleApplication
{
    private readonly IMyService _myService;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ILogger<ConsoleApplication> _logger;

    public ConsoleApplication(IMyService myService, IHostApplicationLifetime appLifetime, ILogger<ConsoleApplication> logger)
    {
        _myService = myService;
        _appLifetime = appLifetime;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Iniciando ConsoleApplication...");

        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    await _myService.DoWorkAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("La operación fue cancelada.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ocurrió un error inesperado.");
                }
                finally
                {
                    _appLifetime.StopApplication();
                }
            });
        });

        await Task.Delay(-1, cancellationToken); // Keep application running until cancellation
    }
}

// Main entry point
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        // Configure User Secrets for development
        builder.Configuration.AddUserSecrets<Program>();

        // Add services to the container.
        builder.Services.AddSingleton<IMyService, MyService>();
        builder.Services.AddSingleton<ConsoleApplication>();

        var host = builder.Build();

        // Retrieve the application from the DI container and run it
        await host.Services.GetRequiredService<ConsoleApplication>().RunAsync();
        
        await host.RunAsync();
    }
}
```

## 3. Notas de Modernización: Beneficios de `field` y Native AOT en .NET 10

### C# 14 y la palabra clave `field`
La introducción de `field` en C# 14 permite una sintaxis más concisa y clara para las propiedades autoimplementadas que necesitan lógica adicional en sus accesores `get` o `init`/`set`. En lugar de declarar explícitamente un campo de respaldo, `field` proporciona una referencia directa al campo generado por el compilador. Esto reduce el *boilerplate* y mejora la legibilidad del código, especialmente en escenarios donde se necesita inicialización condicional o validación en el `set`.

```csharp
public sealed class DataProcessor
{
    // Antes (C# 13 o anterior):
    // private int _maxRetries;
    // public int MaxRetries { get => _maxRetries; init => _maxRetries = value; }

    // Ahora (C# 14 con 'field'):
    public int MaxRetries { get => field; init => field = value; }
}
```
Este enfoque mantiene la compatibilidad con Native AOT ya que no introduce reflexión en tiempo de ejecución.

### Native AOT (Ahead-of-Time) en .NET 10
Native AOT compila la aplicación directamente en código máquina nativo durante el tiempo de publicación. Esto ofrece varios beneficios cruciales para aplicaciones CLI y microservicios:

*   **Rendimiento de Inicio Mejorado**: Las aplicaciones Native AOT se inician significativamente más rápido ya que no hay compilación JIT (Just-In-Time) en tiempo de ejecución, ni carga de JIT, ni sobrecarga de caché de código.
*   **Menor Consumo de Memoria**: Al eliminar el JIT y otros componentes del runtime que no son esenciales para la ejecución nativa, el footprint de memoria de la aplicación se reduce considerablemente.
*   **Binarios Autocontenidos**: Los ejecutables nativos son completamente autocontenidos, eliminando la necesidad de instalar el runtime de .NET en la máquina de destino. Esto simplifica la distribución y el despliegue.
*   **Mayor Seguridad**: La superficie de ataque se reduce al eliminar el JIT y al compilar el código de forma estática, lo que puede ser beneficioso en entornos de "Zero Trust".
*   **Optimización "Physical Promotion" y Desvirtualización**: .NET 10 y sus runtimes modernos implementan optimizaciones avanzadas como "Physical Promotion" (mover objetos del heap al stack cuando es posible) y una mejor desvirtualización de llamadas a métodos de interfaz. Estas técnicas reducen la sobrecarga de la recolección de basura y mejoran el rendimiento de las llamadas a métodos, lo que es especialmente beneficioso en contextos Native AOT.

## 4. Guía de Ejecución: Compilación como Binario Nativo

Para compilar la aplicación `utmMarker` como un binario nativo autocontenido para su sistema operativo actual, utilice el siguiente comando:

```bash
dotnet publish -c Release -r <RID> --sc --no-self-contained -p:PublishAot=true
```

Donde `<RID>` es el identificador de tiempo de ejecución (Runtime Identifier) para su plataforma. Algunos ejemplos comunes son:

*   `win-x64` para Windows de 64 bits.
*   `linux-x64` para Linux de 64 bits.
*   `osx-x64` para macOS de 64 bits (Intel).
*   `osx-arm64` para macOS de 64 bits (Apple Silicon).

**Ejemplo para Windows de 64 bits:**
```bash
dotnet publish -c Release -r win-x64 --sc --no-self-contained -p:PublishAot=true
```

**Explicación de los parámetros:**

*   `-c Release`: Compila en modo "Release" para optimizaciones.
*   `-r <RID>`: Especifica la plataforma de destino.
*   `--sc`: (Self-Contained) Indica que el ejecutable debe ser autocontenido, incluyendo todas las dependencias del runtime. Aunque la salida final no será realmente autocontenida por `PublishAot`, este flag asegura que todas las dependencias necesarias se resuelvan correctamente.
*   `--no-self-contained`: Por defecto, `PublishAot=true` implica `--self-contained`. Sin embargo, para .NET 10, es común usar `--no-self-contained` con `PublishAot=true` cuando se desea un ejecutable realmente nativo que no incluya el runtime de .NET, pero sí las bibliotecas nativas necesarias. Esto resulta en un tamaño de binario más pequeño.
*   `-p:PublishAot=true`: Habilita la publicación Ahead-of-Time (AOT), que genera un ejecutable nativo.

Después de la ejecución, el binario nativo se encontrará en `bin\Release
et10.0\<RID>\publish`.
