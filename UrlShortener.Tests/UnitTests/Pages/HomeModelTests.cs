using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using UrlShortener.Pages;
using UrlShortener.Services;

namespace UrlShortener.Tests.UnitTests.Pages
{
    public class HomeModelTests
    {
        private readonly Mock<ILinkService> _mockLinkService;
        private readonly Mock<IUrlValidationService> _mockValidator;
        private readonly HomeModel _pageModel;

        public HomeModelTests()
        {
            _mockLinkService = new Mock<ILinkService>();
            _mockValidator = new Mock<IUrlValidationService>();
            _pageModel = new HomeModel(_mockLinkService.Object, _mockValidator.Object);
        }

        [Fact]
        public async Task OnPostAsync_ValidUrl_CreatesLinkAndRedirects()
        {
            // Arrange
            _pageModel.NewUrl = "valid-url.com";
            _mockValidator.Setup(v => v.NormalizeUrl(It.IsAny<string>())).Returns("https://valid-url.com");
            _mockValidator.Setup(v => v.IsValidUrl(It.IsAny<string>())).Returns(true);

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            _mockLinkService.Verify(s => s.CreateLinkAsync("https://valid-url.com"), Times.Once);
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(1, redirect.RouteValues?["pageNumber"]);
        }

        [Fact]
        public async Task OnPostAsync_InvalidUrl_ReturnsPageWithError()
        {
            // Arrange
            _pageModel.NewUrl = "bad-url";
            _mockValidator.Setup(v => v.NormalizeUrl(It.IsAny<string>())).Returns("bad-url");
            _mockValidator.Setup(v => v.IsValidUrl(It.IsAny<string>())).Returns(false);

            // Act
            var result = await _pageModel.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(_pageModel.ModelState.IsValid);
            Assert.True(_pageModel.ModelState.ContainsKey("NewUrl"));

            _mockLinkService.Verify(s => s.CreateLinkAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task OnGetDeleteAsync_DeletesAndRedirectsCorrectly()
        {
            // Arrange
            int idToDelete = 10;
            int currentPage = 2;

            _mockLinkService.Setup(s => s.GetTotalCountAsync()).ReturnsAsync(5);
            _mockLinkService.Setup(s => s.CorrectPageNumber(currentPage, 5, It.IsAny<int>())).Returns(1);

            // Act
            var result = await _pageModel.OnGetDeleteAsync(idToDelete, currentPage);

            // Assert
            _mockLinkService.Verify(s => s.DeleteLinkAsync(idToDelete), Times.Once);
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(1, redirect.RouteValues?["pageNumber"]);
        }
    }
}
