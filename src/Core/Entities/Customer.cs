using System.Text.RegularExpressions;

namespace utmMarker.Core.Entities;

public class Customer
{
    private string _email = string.Empty;

    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email
    {
        get => _email;
        set
        {
            if (!IsValidEmail(value))
            {
                throw new ArgumentException("Invalid email format.");
            }
            _email = value;
        }
    }
    public bool IsActive { get; set; }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        // Simplified regex for demonstration. A more robust regex might be needed for production.
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    }
}
