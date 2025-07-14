namespace Listings.Shared
{
    public static class ErrorMessages
    {
        public const string NotFound = "No listing found for the specified address";
        public const string InternalServerError = "An error occurred while retrieving listing information";
    }

    public static class CacheKeys
    {
        public const string AddressPrefix = "Address_";
    }
}