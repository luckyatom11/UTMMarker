using utmMarker.Core.Entities;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public class CreateCustomerUseCaseImpl(ICustomerRepository customerRepository, IEmailValidationUseCase emailValidationUseCase) : ICreateCustomerUseCase
{
    public async Task ExecuteAsync(string fullName, string email)
    {
        if (!await emailValidationUseCase.ValidateAsync(email))
        {
            throw new ArgumentException("Invalid email format.");
        }

        if (!await emailValidationUseCase.IsEmailUniqueAsync(email))
        {
            throw new ArgumentException("Email already exists.");
        }

        var customer = new Customer
        {
            FullName = fullName,
            Email = email,
            IsActive = true // New customers are active by default
        };

        await customerRepository.AddAsync(customer);
    }
}
