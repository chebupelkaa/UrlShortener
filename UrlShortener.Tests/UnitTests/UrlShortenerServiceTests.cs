using System;
using System.Linq;
using Xunit;
using UrlShortener.Services;

namespace UrlShortener.Tests.UnitTests;

public class UrlShortenerServiceTests
{
    private readonly UrlShortenerService _service;

    public UrlShortenerServiceTests()
    {
        _service = new UrlShortenerService();
    }

    [Fact]
    public void GenerateUniqueCode_ShouldReturnStringOfCorrectLength()
    {
        // Act
        var result = _service.GenerateUniqueCode();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(7, result.Length); // CodeLength = 7
    }

    [Fact]
    public void GenerateUniqueCode_ShouldOnlyContainAllowedCharacters()
    {
        // Act
        var result = _service.GenerateUniqueCode();
        var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        // Assert
        foreach (char c in result)
        {
            Assert.Contains(c, allowedChars);
        }
    }

    [Fact]
    public void GenerateUniqueCode_ShouldGenerateDifferentCodesOnMultipleCalls()
    {
        // Act
        var code1 = _service.GenerateUniqueCode();
        var code2 = _service.GenerateUniqueCode();
        var code3 = _service.GenerateUniqueCode();

        // Assert
        Assert.NotEqual(code1, code2);
        Assert.NotEqual(code1, code3);
        Assert.NotEqual(code2, code3);
    }

    [Fact]
    public void GenerateUniqueCode_ShouldBeRandomEnough_NoObviousPatterns()
    {
        // Act
        var codes = Enumerable.Range(0, 100)
            .Select(_ => _service.GenerateUniqueCode())
            .ToList();

        // Assert
        var uniqueCodes = codes.Distinct().Count();
        Assert.True(uniqueCodes > 95, $"Expected >95 unique codes out of 100, got {uniqueCodes}");
    }

    [Fact]
    public void GenerateUniqueCode_ShouldUseCryptographicRandomness()
    {
        // Просто проверяем, что метод не падает и возвращает разные значения
        // Это косвенная проверка, что RandomNumberGenerator работает

        var results = new HashSet<string>();

        for (int i = 0; i < 1000; i++)
        {
            var code = _service.GenerateUniqueCode();
            results.Add(code);
        }

        // Проверяем, что коллизий нет
        Assert.Equal(1000, results.Count);
    }
}