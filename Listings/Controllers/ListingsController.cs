using Listings.Models;
using Listings.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Listings.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingsController(IListingInfoService listingInfoService, ILogger<ListingsController> logger) : ControllerBase
{
    [HttpGet("{address}")]
    public async Task<ActionResult> Get(string address)
    {
        logger.LogInformation($"Received request for Address - {address}");
        var listingResponse = await listingInfoService.GetListingInfo(address);

        if (listingResponse == null)
        {
            logger.LogInformation($"Address not found - {address}");
            return NotFound(new ListingResponse { Error = Shared.ErrorMessages.NotFound, Address = address });
        }
        
        if (!listingResponse.IsSuccess)
        {
            logger.LogError($"Error occurred for address - {address}");
            return StatusCode(500, listingResponse);
        }
        
        logger.LogInformation($"Successfully retrieved property info for Address - {address}");
        return Ok(listingResponse.PropertyInfo);
    }
}