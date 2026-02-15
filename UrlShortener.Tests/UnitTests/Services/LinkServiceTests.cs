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
        public async Task DeleteLinkAsync_RemovesItem()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var link = new ShortLink { Id = 99, OriginalUrl = "test", ShortCode = "test" };
            db.ShortLinks.Add(link);
            await db.SaveChangesAsync();

            var service = new LinkService(db, null!);

            // Act
            var isDeleted = await service.DeleteLinkAsync(99);

            // Assert
            Assert.True(isDeleted);
            Assert.Empty(db.ShortLinks);
        }

        [Fact]
        public void CorrectPageNumber_AdjustsInvalidPages()
        {
            var service = new LinkService(null!, null!);

            // Act & Assert
            Assert.Equal(5, service.CorrectPageNumber(10, 50, 10));

            Assert.Equal(1, service.CorrectPageNumber(-1, 50, 10));

            Assert.Equal(2, service.CorrectPageNumber(2, 50, 10));
        }

        [Fact]
        public async Task GetLinkByIdAsync_ExistingId_ReturnsLink()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var link = new ShortLink { Id = 10, OriginalUrl = "find-me", ShortCode = "find" };
            db.ShortLinks.Add(link);
            await db.SaveChangesAsync();

            var service = new LinkService(db, null!);

            // Act
            var result = await service.GetLinkByIdAsync(10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Id);
            Assert.Equal("find-me", result.OriginalUrl);
        }

        [Fact]
        public async Task GetLinkByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            using var db = GetInMemoryDbContext();
            var service = new LinkService(db, null!);

            // Act
            var result = await service.GetLinkByIdAsync(999);

            // Assert
            Assert.Null(result);
        }
    }
}
