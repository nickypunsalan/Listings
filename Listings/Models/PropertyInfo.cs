using System.Text.Json.Serialization;

namespace Listings.Models;

public class PropertyInfo
{
    public int ListingId { get; set; }
    public required string Address { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int SquareFootage { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Description { get; set; }
}