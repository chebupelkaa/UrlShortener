using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using UrlShortener.Pages;
using UrlShortener.Services;

namespace UrlShortener.Tests.UnitTests.Pages
{
    public class HomeModelTests
    {
        private readonly Mock<ILinkService> _linkServiceMock;
        private readonly Mock<IUrlValidationService> _validatorMock;
        private readonly HomeModel _model;

        public HomeModelTests()
        {
            _linkServiceMock = new Mock<ILinkService>();
            _validatorMock = new Mock<IUrlValidationService>();
            _model = new HomeModel(_linkServiceMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task OnPostAsync_ValidUrl_CreatesLinkAndRedirectsToPage1()
        {
            // Arrange
            _model.NewUrl = "valid.com";

            _validatorMock.Setup(v => v.NormalizeUrl(It.IsAny<string>())).Returns("https://valid.com");
            _validatorMock.Setup(v => v.IsValidUrl(It.IsAny<string>())).Returns(true);

            // Act
            var result = await _model.OnPostAsync();

            // Assert
            _linkServiceMock.Verify(s => s.CreateLinkAsync("https://valid.com"), Times.Once);

            var redirectResult = Assert.IsType<RedirectToPageResult>(result);

            Assert.Equal(1, redirectResult.RouteValues?["pageNumber"]);
        }

        [Fact]
        public async Task OnPostAsync_InvalidUrl_ReturnsPageWithError()
        {
            // Arrange
            _model.NewUrl = "bad_input";

            _validatorMock.Setup(v => v.NormalizeUrl(It.IsAny<string>())).Returns("bad_input");
            _validatorMock.Setup(v => v.IsValidUrl(It.IsAny<string>())).Returns(false);

            // Act
            var result = await _model.OnPostAsync();

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.False(_model.ModelState.IsValid);
            Assert.True(_model.ModelState.ContainsKey("NewUrl"));

            _linkServiceMock.Verify(s => s.CreateLinkAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task OnGetDeleteAsync_DeletesAndRedirectsBack()
        {
            // Arrange
            int idToDelete = 5;
            int currentPage = 2;

            _linkServiceMock.Setup(s => s.DeleteLinkAsync(idToDelete)).ReturnsAsync(true);
            _linkServiceMock.Setup(s => s.GetTotalCountAsync()).ReturnsAsync(4);
            _linkServiceMock.Setup(s => s.CalculateTotalPages(4, 5)).Returns(1);

            _model.PageSize = 5;

            // Act
            var result = await _model.OnGetDeleteAsync(idToDelete, currentPage);

            // Assert
            var redirectResult = Assert.IsType<RedirectToPageResult>(result);

            Assert.Equal(1, redirectResult.RouteValues?["pageNumber"]);
        }

        [Fact]
        public async Task OnPostUpdateAsync_ValidUrl_UpdatesAndRedirects()
        {
            // Arrange
            int idToUpdate = 1;
            string newUrl = "updated.com";
            int currentPage = 3;

            _validatorMock.Setup(v => v.NormalizeUrl(newUrl)).Returns("https://updated.com");
            _validatorMock.Setup(v => v.IsValidUrl(It.IsAny<string>())).Returns(true);

            // Act
            var result = await _model.OnPostUpdateAsync(idToUpdate, newUrl, currentPage);

            // Assert
            _linkServiceMock.Verify(s => s.UpdateLinkAsync(idToUpdate, "https://updated.com"), Times.Once);

            var redirectResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(currentPage, redirectResult.RouteValues?["pageNumber"]);
        }
    }
}
