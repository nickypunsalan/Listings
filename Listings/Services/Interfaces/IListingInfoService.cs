using Listings.Models;

namespace Listings.Services.Interfaces;

public interface IListingInfoService
{
    Task<ListingResponse?> GetListingInfo(string address);
}