namespace utmMarker.Core.UseCases;

public interface IEmailValidationUseCase
{
    Task<bool> ValidateAsync(string email);
    Task<bool> IsEmailUniqueAsync(string email);
}
