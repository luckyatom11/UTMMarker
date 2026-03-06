using System.Text.RegularExpressions;
using utmMarker.Core.Repositories;
using utmMarker.Core.UseCases;

namespace utmMarker.Application;

public class EmailValidationUseCaseImpl(ICustomerRepository customerRepository) : IEmailValidationUseCase
{
    public Task<bool> ValidateAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult(false);
        }
        // Basic regex for email format validation
        bool isValidFormat = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        return Task.FromResult(isValidFormat);
    }

    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var customer = await customerRepository.GetByEmailAsync(email);
        return customer == null;
    }
}
