using System.Security.Cryptography;

namespace UrlShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int CodeLength = 7;

        public string GenerateUniqueCode()
        {
            // через stackalloc выделяем память на стеке, а не в куче
            Span<byte> randomBytes = stackalloc byte[CodeLength];
            Span<char> result = stackalloc char[CodeLength];

            RandomNumberGenerator.Fill(randomBytes);

            for (int i = 0; i < CodeLength; i++)
            {
                result[i] = Alphabet[randomBytes[i] % Alphabet.Length];
            }

            return new string(result);
        }
    }
}
