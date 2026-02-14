using Microsoft.EntityFrameworkCore;
using Moq;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Services;

namespace UrlShortener.Tests.UnitTests.Services
{
    public class LinkServiceTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task CreateLinkAsync_SavesToDatabase()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var generatorMock = new Mock<IUrlShortenerService>();
            generatorMock.Setup(s => s.GenerateUniqueCode()).Returns("ABC1234");

            var service = new LinkService(db, generatorMock.Object);

            // Act
            var result = await service.CreateLinkAsync("https://test.com");

            // Assert
            var saved = await db.ShortLinks.FirstOrDefaultAsync();
            Assert.NotNull(saved);
            Assert.Equal("ABC1234", saved.ShortCode);
            Assert.Equal("https://test.com", saved.OriginalUrl);
        }

        [Fact]
        public async Task GetPagedLinksAsync_ReturnsNewestFirst()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            // Добавляем старую запись
            db.ShortLinks.Add(new ShortLink { CreatedAt = DateTime.UtcNow.AddDays(-1), ShortCode = "OLD", OriginalUrl = "old" });
            // Добавляем новую запись
            db.ShortLinks.Add(new ShortLink { CreatedAt = DateTime.UtcNow, ShortCode = "NEW", OriginalUrl = "new" });
            await db.SaveChangesAsync();

            var service = new LinkService(db, null!);

            // Act
            var result = await service.GetPagedLinksAsync(1, 10);

            // Assert
            Assert.Equal("NEW", result.Links.First().ShortCode);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task DeleteLinkAsync_RemovesItem()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var link = new ShortLink { Id = 1, ShortCode = "DEL", OriginalUrl = "url" };
            db.ShortLinks.Add(link);
            await db.SaveChangesAsync();

            var service = new LinkService(db, null!);

            // Act
            var result = await service.DeleteLinkAsync(1);

            // Assert
            Assert.True(result);
            Assert.Empty(db.ShortLinks);
        }

        [Fact]
        public void CalculateTotalPages_WorksCorrectly()
        {
            var service = new LinkService(null!, null!);

            // Act & Assert
            Assert.Equal(3, service.CalculateTotalPages(11, 5));
            Assert.Equal(2, service.CalculateTotalPages(10, 5));
            Assert.Equal(0, service.CalculateTotalPages(0, 5));
        }
    }
}
