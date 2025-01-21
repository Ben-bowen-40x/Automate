using Automate.Application.InfrastructureInterfaces;

namespace Automate.Translation.InfrastructureInterfaces.JsonToCsv;

public interface IWebFormElem
{
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
public interface IDateString
{
    public string? Date { get; set; }
}
public interface IDateTime
{
    public DateTime Date { get; set; }
}
public interface IWebFormString : IDateString, IWebFormElem { }
public interface IWebFormTyped : IDateTime, IWebFormElem, IPhoneNumberCompatible { }