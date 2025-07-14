using FluentAssertions;
using Listings.Models;
using Listings.Services;
using Moq;

namespace Listings.Test.Services;

public class ListingInfoServiceTests
{
    private Mock<IPropertyInfoApi> _mockPropertyInfoApi;
    private ListingInfoService _listingInfoService;

    [SetUp]
    public void Setup()
    {
        _mockPropertyInfoApi = new Mock<IPropertyInfoApi>();
        _listingInfoService = new ListingInfoService(_mockPropertyInfoApi.Object);
    }

    [Test]
    [TestCase(TestData.Addresses.ValidAddresses.ValidAddress1)]
    [TestCase(TestData.Addresses.ValidAddresses.ValidAddress2)]
    [TestCase(TestData.Addresses.ValidAddresses.ValidAddress3)]
    public async Task WhenAddressIsValid_ReturnValidListingResponse(string address)
    {
        _mockPropertyInfoApi
            .Setup(m => m.GetPropertyInfo(address))
            .ReturnsAsync(new PropertyInfo
            {
                Address = address
            });
        
        var result = await _listingInfoService.GetListingInfo(address);

        result.Should().NotBeNull();
        result.PropertyInfo.Should().NotBeNull();
        result.Address.Should().BeNull();
        result.Error.Should().BeNull();
    }

    [Test]
    [TestCase("")]
    [TestCase(TestData.Addresses.InvalidAddresses.InvalidAddress1)]
    public async Task WhenAddressIsInvalid_ReturnNullListingResponse(string address)
    {
        _mockPropertyInfoApi
            .Setup(m => m.GetPropertyInfo(address))
            .ReturnsAsync((PropertyInfo?)null);
        
        var result = await _listingInfoService.GetListingInfo(address);

        result.Should().BeNull();
    }

    [Test]
    [TestCase(TestData.Addresses.ValidAddresses.ValidAddress1)]
    public async Task WhenPropertyApi_ThrowsException_ReturnsErrorMessage(string address)
    {
        _mockPropertyInfoApi.Setup(m => m.GetPropertyInfo(address)).Throws<Exception>();
        
        var result = await _listingInfoService.GetListingInfo(address);
        result.Should().NotBeNull();
        result.Error.Should().Be("An error occurred while retrieving listing information");
    }
}