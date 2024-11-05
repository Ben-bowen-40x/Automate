using System.Text.Json.Serialization;

namespace Automate.Infrastructure.JsonToCsvService.JsonMaps;

public class JsonMessage
{
    [JsonPropertyName("Date")]
    public string? Date { get; set; }
    [JsonPropertyName("First Name")]
    public string? FirstName { get; set; }
    [JsonPropertyName("Last Name")]
    public string? LastName { get; set; }
    [JsonPropertyName("Phone")]
    public string? Phone { get; set; }
    [JsonPropertyName("Email")]
    public string? Email { get; set; }
    [JsonPropertyName("Problem")]
    public string? Problem { get; set; }
    [JsonPropertyName("Branch")]
    public string? Branch { get; set; }
    [JsonPropertyName("Referring URL")]
    public string? ReferringUrl { get; set; }
    [JsonPropertyName("Form Page URL")]
    public string? FormPageUrl { get; set; }
    [JsonPropertyName("Current Customer")]
    public string? CurrentCustomer { get; set; }
    [JsonPropertyName("Zip")]
    public string? Zip { get; set; }
}
