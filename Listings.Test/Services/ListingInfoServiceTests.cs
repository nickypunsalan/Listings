using FluentAssertions;
using Listings.Models;
using Listings.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Listings.Test.Services;

public class ListingInfoServiceTests
{
    private Mock<IPropertyInfoApi> _mockPropertyInfoApi;
    private Mock<IMemoryCache> _mockMemoryCache;
    private ListingInfoService _listingInfoService;

    [SetUp]
    public void Setup()
    {
        _mockPropertyInfoApi = new Mock<IPropertyInfoApi>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        
        object propertyInfo = null!;
        _mockMemoryCache.Setup(m => m.TryGetValue(It.IsAny<string>(), out propertyInfo!))
            .Returns(false);
        
        var mockCacheEntry = new Mock<ICacheEntry>();
        _mockMemoryCache.Setup(m => m.CreateEntry(It.IsAny<string>()))
            .Returns(mockCacheEntry.Object);
        
        _listingInfoService = new ListingInfoService(
            _mockPropertyInfoApi.Object,
            _mockMemoryCache.Object,
            NullLogger<ListingInfoService>.Instance);
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
        result.Error.Should().Be(Shared.ErrorMessages.InternalServerError);
    }

    [Test]
    [TestCase(TestData.Addresses.ValidAddresses.ValidAddress1)]
    public async Task WhenCacheEntryForProperty_Exists_ReturnCachedData(string address)
    {
        var propertyInfo = new PropertyInfo { Address = address };
        
        _mockMemoryCache
            .Setup(c => c.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny!))
            .Callback((object key, out object value) =>
            {
                value = propertyInfo;
            })
            .Returns(true);
        
        var result = await _listingInfoService.GetListingInfo(address);

        result.Should().NotBeNull();
        _mockMemoryCache.Verify(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny!), Times.Once);
    }

    [Test]
    [TestCase(TestData.Addresses.ValidAddresses.ValidAddress1)]
    public async Task WhenCacheEntryForProperty_DoesNotExist_CallApi_And_StoreResultInCache(string address)
    {
        _mockMemoryCache
            .Setup(c => c.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny!))
            .Callback((object key, out object value) =>
            {
                value = null!;
            })
            .Returns(true);
        
        _mockPropertyInfoApi
            .Setup(m => m.GetPropertyInfo(address))
            .ReturnsAsync(new PropertyInfo
            {
                Address = address
            });
        
        var result = await _listingInfoService.GetListingInfo(address);
        
        result.Should().NotBeNull();
        _mockMemoryCache.Verify(m => m.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny!), Times.Once);
        _mockPropertyInfoApi.Verify(m => m.GetPropertyInfo(address), Times.Once);
        _mockMemoryCache.Verify(m => m.CreateEntry(It.IsAny<object>()), Times.Once);
    }
}