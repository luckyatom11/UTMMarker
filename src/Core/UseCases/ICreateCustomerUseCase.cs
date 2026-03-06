namespace utmMarker.Core.UseCases;

public interface ICreateCustomerUseCase
{
    Task ExecuteAsync(string fullName, string email);
}
