using System.Web;
using Listings.Models;
using Listings.Services.Interfaces;

namespace Listings.Services;

public class ListingInfoService(IPropertyInfoApi propertyInfoApi, ILogger<ListingInfoService> logger) : IListingInfoService
{
    public async Task<ListingResponse?> GetListingInfo(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            logger.LogInformation("Was not provided an address. Returning null.");
            return null;
        }
        
        var cleanedAddress = HttpUtility.UrlDecode(address);
        
        try
        {
            var propertyInfo = await propertyInfoApi.GetPropertyInfo(cleanedAddress);

            if (propertyInfo == null)
            {
                logger.LogInformation("Did not find property. Returning null.");
                return null;
            }

            logger.LogInformation($"Found ListingId - {propertyInfo.ListingId} | Address - {address}");
            return new ListingResponse
            {
                PropertyInfo = propertyInfo
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Error getting listing info for Address - {address}");
            return new ListingResponse
            {
                Error = Shared.ErrorMessages.InternalServerError,
                IsSuccess = false
            };
        }
    }
}