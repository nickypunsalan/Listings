using System.Web;
using Listings.Models;
using Listings.Services.Interfaces;

namespace Listings.Services;

public class ListingInfoService(IPropertyInfoApi propertyInfoApi) : IListingInfoService
{
    public async Task<ListingResponse?> GetListingInfo(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return null;
        }
        
        var cleanedAddress = HttpUtility.UrlDecode(address);
        
        try
        {
            var propertyInfo = await propertyInfoApi.GetPropertyInfo(cleanedAddress);

            if (propertyInfo == null)
            {
                return null;
            }

            return new ListingResponse
            {
                PropertyInfo = propertyInfo
            };
        }
        catch (Exception ex)
        {
            return new ListingResponse
            {
                Error = Shared.ErrorMessages.InternalServerError,
                IsSuccess = false
            };
        }
    }
}