using System.Web;
using Listings.Models;
using Listings.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Listings.Services;

public class ListingInfoService(
    IPropertyInfoApi propertyInfoApi,
    IMemoryCache memoryCache,
    ILogger<ListingInfoService> logger) : IListingInfoService
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
            PropertyInfo? propertyInfo = CheckCache(cleanedAddress);
            if (propertyInfo != null)
            {
                return new ListingResponse
                {
                    PropertyInfo = propertyInfo
                }; 
            }
            
            propertyInfo = await propertyInfoApi.GetPropertyInfo(cleanedAddress);

            if (propertyInfo == null)
            {
                logger.LogInformation("Did not find property. Returning null.");
                return null;
            }

            logger.LogInformation($"Found ListingId - {propertyInfo.ListingId} | Address - {address}");
            AddCacheData(cleanedAddress, propertyInfo);
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
    
    private PropertyInfo? CheckCache(string cleanedAddress)
    {
        var cacheKey = $"{Shared.CacheKeys.AddressPrefix}{cleanedAddress}";
        logger.LogInformation($"Cache: Checking for the following key - {cacheKey}");
        
        if (memoryCache.TryGetValue($"{cacheKey}",
                out PropertyInfo? propertyInfo))
        {
            logger.LogInformation($"Cache: Retrieved property info for address - {cleanedAddress} from cache.");
            return propertyInfo;
        }

        logger.LogInformation($"Cache: Key - {cacheKey} not found");
        return null;
    }

    private void AddCacheData(string cleanedAddress, PropertyInfo propertyInfo)
    {
        var cacheKey = $"{Shared.CacheKeys.AddressPrefix}{cleanedAddress}";
        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(1));

        memoryCache.Set($"{cacheKey}", propertyInfo, cacheEntryOptions);
        logger.LogInformation($"Cache: Stored property info with the following cache key - {cacheKey}");
    }
}