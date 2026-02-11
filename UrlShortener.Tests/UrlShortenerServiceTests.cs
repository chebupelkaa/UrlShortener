using System;
using System.Linq;
using Xunit;
using UrlShortener.Services;

namespace UrlShortener.Tests;

public class UrlShortenerServiceTests
{
    [Fact]
    public void GenerateUniqueCode_ReturnsCorrectLength()
    {
        // Arrange
        var service = new UrlShortenerService();

        // Act
        var code = service.GenerateUniqueCode();

        // Assert
        Assert.Equal(7, code.Length);
    }

    [Fact]
    public void GenerateUniqueCode_ContainsOnlyValidChars()
    {
        // Arrange
        var service = new UrlShortenerService();
        var validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // Act
        var code = service.GenerateUniqueCode();

        // Assert
        Assert.All(code, c => Assert.Contains(c, validChars));
    }

    [Fact]
    public void GenerateUniqueCode_IsRandom()
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