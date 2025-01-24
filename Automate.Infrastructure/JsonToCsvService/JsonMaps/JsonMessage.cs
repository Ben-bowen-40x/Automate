using Automate.Infrastructure.MessageLeadsService.DbMaps;
using Automate.Translation.ContactTranslate;
using System.Text.Json.Serialization;

namespace Automate.Infrastructure.JsonToCsvService.JsonMaps;

public class JsonMessage: IContactFormString
{
    [JsonPropertyName(WebFormEntity.DateStr)]
    public string? Date { get; set; }
    [JsonPropertyName(WebFormEntity.FName)]
    public string? FirstName { get; set; }
    [JsonPropertyName(WebFormEntity.LName)]
    public string? LastName { get; set; }
    [JsonPropertyName(WebFormEntity.Ph)]
    public string? Phone { get; set; }
    [JsonPropertyName(WebFormEntity.Eml)]
    public string? Email { get; set; }
    [JsonPropertyName(WebFormEntity.Prob)]
    public string? Problem { get; set; }
    [JsonPropertyName(WebFormEntity.Brnch)]
    public string? Branch { get; set; }
    [JsonPropertyName(WebFormEntity.Refer)]
    public string? ReferringUrl { get; set; }
    [JsonPropertyName(WebFormEntity.Form)]
    public string? FormPageUrl { get; set; }
    [JsonPropertyName(WebFormEntity.Cust)]
    public string? CurrentCustomer { get; set; }
    [JsonPropertyName(WebFormEntity.ZCode)]
    public string? Zip { get; set; }
}
