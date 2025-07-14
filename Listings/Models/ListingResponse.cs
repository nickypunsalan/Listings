using System.Text.Json.Serialization;

namespace Listings.Models;

public class ListingResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PropertyInfo? PropertyInfo { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool IsSuccess { get; set; } = true;
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Address { get; set; }
}