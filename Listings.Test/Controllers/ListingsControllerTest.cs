using FluentAssertions;
using Listings.Controllers;
using Listings.Models;
using Listings.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Listings.Test.Controllers;

public class ListingsControllerTest
{
    private Mock<IListingInfoService> _mockListingInfoService;
    private ListingsController _listingsController;

    [SetUp]
    public void Setup()
    {
        _mockListingInfoService = new Mock<IListingInfoService>();
        _listingsController = new ListingsController(_mockListingInfoService.Object);
    }

    [Test]
    [TestCase(TestData.Addresses.ValidAddresses.ValidAddress1)]
    public async Task WhenAddressIsValid_ReturnOk(string address)
    {
        _mockListingInfoService.Setup(m => m.GetListingInfo(address))
            .ReturnsAsync(new ListingResponse());

        var result = await _listingsController.Get(address);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Test]
    [TestCase(TestData.Addresses.InvalidAddresses.InvalidAddress1)]
    public async Task WhenAddressIsNotFound_ReturnNotFound(string address)
    {
        _mockListingInfoService.Setup(m => m.GetListingInfo(address))
            .ReturnsAsync((ListingResponse?)null);

        var result = await _listingsController.Get(address);
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    [TestCase(TestData.Addresses.ValidAddresses.ValidAddress1)]
    public async Task WhenExceptionOccurs_ReturnInternalServerError(string address)
    {
        _mockListingInfoService.Setup(m => m.GetListingInfo(address))
            .ReturnsAsync(new ListingResponse
            {
                Error = Shared.ErrorMessages.InternalServerError,
                IsSuccess = false
            });

        var result = await _listingsController.Get(address);
        result.Should().BeOfType<ObjectResult>(Shared.ErrorMessages.InternalServerError);
    }
}