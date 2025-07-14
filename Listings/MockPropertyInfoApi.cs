using Listings.Models;

namespace Listings;

public class MockPropertyInfoApi : IPropertyInfoApi
{
    private readonly List<PropertyInfo> _properties =
    [
        new()
        {
        ListingId = 12345,
        Address = "123 Main Street, Sydney NSW 2000",
        Bedrooms = 3,
        Bathrooms = 2,
        SquareFootage = 1200,
        Description = "Beautiful family home in prime location"
        },
        new()
        {
        ListingId = 67890,
        Address = "456 Oak Avenue, Melbourne VIC 3000",
        Bedrooms = 4,
        Bathrooms = 3,
        SquareFootage = 1800
        },
        new()
        {
        ListingId = 11111,
        Address = "789 Pine Road, Brisbane QLD 4000",
        Bedrooms = 2,
        Bathrooms = 1,
        SquareFootage = 900
        }
    ];

    public async Task<PropertyInfo?> GetPropertyInfo(string address)
    {
        var result = _properties.FirstOrDefault(p => p.Address.Equals(address));
        if (result == null)
        {
            return null;
        }

        return await Task.FromResult(result);
    }
}