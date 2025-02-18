using Automate.Application.InfrastructureInterfaces;

namespace Automate.Translation.ContactFormTranslate;

public interface IContactFormTyped : IPhoneNumberCompatible
{
    DateTime Date { get; set; }
    string? FirstName { get; set; }
    string? LastName { get; set; }
    string? Phone { get; set; }
    string? Email { get; set; }
    string? Problem { get; set; }
    string? Branch { get; set; }
    string? ReferringUrl { get; set; }
    string? FormPageUrl { get; set; }
    string? CurrentCustomer { get; set; }
    string? Zip { get; set; }
}
