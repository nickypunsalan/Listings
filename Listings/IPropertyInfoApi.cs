using Listings.Models;

namespace Listings;

public interface IPropertyInfoApi
{
   Task<PropertyInfo?> GetPropertyInfo(string address);
}