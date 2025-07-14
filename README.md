# Listings
Listings is an ASP.NET Core Web API that retrieves property listing information based on user-provided addresses.

## Installation
1. Clone the Listings repo - `git clone https://github.com/nickypunsalan/Listings.git`

2. Open the solution `.sln` file. You can do this through either Visual Studio or JetBrains Rider IDE applications.
3. Ensure configuration is set to `http` for local development
4. Run the Web API. This will start it on - `http://localhost:5210`

## Usage

### Endpoint
`GET /api/listings/{address}`

Retrieves property info by a single address
- Method: `GET`
- Path: `/api/listings/{address}`
- URL Parameter:
  - `address` (string) - user-provided address 

### Request example
```
GET /api/listings?address=123%20Main%20Street,%20Sydney%20NSW%202000
```

### Response Examples

**Success (200 OK):**

```json
{
  "listingId": 12345,
  "address": "123 Main Street, Sydney NSW 2000",
  "bedrooms": 3,
  "bathrooms": 2,
  "squareFootage": 1200,
  "description": "Beautiful family home in prime location"
}
```

**Not Found (404 Not Found):**

```json
{
  "error": "No listing found for the specified address",
  "address": "999 Unknown Street"
}
```

**Server Error (500 Internal Server Error):**

```json
{
  "error": "An error occurred while retrieving listing information"
}
```

## Architecture

- Separated concerns using layered architecture, making the code base easier to understand, develop and modify
  - API Layer: Controllers responsible for handling HTTP request and translating business results into API responses.
  - Business Logic layer: Services which containing the core business rules and orchestrating operations.
  - Models layer: Models representing the application's data entities.
  

- Loose coupling and interfaces
  - Adheres to the Dependency Injection design pattern 
  - Allows for services to be mocked for testing


- Test project which follows the same folder structure as the application code


- Constant strings for reusability

## Bonus Features (Optional)
- Implemented a simple memory cache to demonstrate caching strategy


## Assumptions

- `MockPropertyInfoApi.GetListingInfo()` returns a single PropertyInfo, not a collection of PropertyInfo
- Address is a unique value / there cannot be duplicates of the same address.
- Address values may be provided as an encoded string. The address value is decoded before passing it to `MockPropertyInfoApi`.
- By default, the Web API requires an address value to be provided. A `IsNullOrWhiteSpace` check is still used as a safeguard.