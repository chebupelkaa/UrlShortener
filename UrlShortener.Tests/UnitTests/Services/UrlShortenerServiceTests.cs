using UrlShortener.Services;

namespace UrlShortener.Tests.UnitTests.Services
{
    public class UrlShortenerServiceTests
    {
        [Fact]
        public void GenerateUniqueCode_ReturnsStringWithCorrectLength()
        {
            // Arrange
            var service = new UrlShortenerService();

            // Act
            var code = service.GenerateUniqueCode();

            // Assert
            Assert.Equal(7, code.Length);
        }
        [Fact]
        public void GenerateUniqueCode_ReturnsDifferentCodes()
        {
            // Arrange
            var service = new UrlShortenerService();

            // Act
            var code1 = service.GenerateUniqueCode();
            var code2 = service.GenerateUniqueCode();

            // Assert
            Assert.NotEqual(code1, code2);
        }
    }
}
