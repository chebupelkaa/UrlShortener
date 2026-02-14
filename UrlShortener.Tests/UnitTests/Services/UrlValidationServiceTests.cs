using UrlShortener.Services;

namespace UrlShortener.Tests.UnitTests.Services
{
    public class UrlValidationTests
    {
        private readonly UrlValidationService _service = new();

        [Fact]
        public void NormalizeUrl_AddsHttps()
        {
            var res = _service.NormalizeUrl("google.com");
            Assert.Equal("https://google.com", res);
        }

        [Fact]
        public void IsValidUrl_RejectsGarbage()
        {
            Assert.True(_service.IsValidUrl("https://google.com"));
            Assert.False(_service.IsValidUrl("just text"));
            Assert.False(_service.IsValidUrl("javascript:alert(1)"));
        }
    }
}

