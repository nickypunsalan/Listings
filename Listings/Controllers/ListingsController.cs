using Listings.Models;
using Listings.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Listings.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ListingsController(IListingInfoService listingInfoService) : ControllerBase
{
    [HttpGet("{address}")]
    public async Task<ActionResult> Get(string address)
    {
        var listingResponse = await listingInfoService.GetListingInfo(address);

        if (listingResponse == null)
        {
            return NotFound(new ListingResponse { Error = Shared.ErrorMessages.NotFound, Address = address });
        }
        
        if (!listingResponse.IsSuccess)
        {
            return StatusCode(500, listingResponse);
        }
        
        return Ok(listingResponse.PropertyInfo);
    }
}