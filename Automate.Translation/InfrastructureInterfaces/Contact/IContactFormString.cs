namespace Automate.Translation.InfrastructureInterfaces.Contact;

public interface IContactFormString
{
    public string? Date { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Problem { get; set; }
    public string? Branch { get; set; }
    public string? ReferringUrl { get; set; }
    public string? FormPageUrl { get; set; }
    public string? CurrentCustomer { get; set; }
    public string? Zip { get; set; }
}
