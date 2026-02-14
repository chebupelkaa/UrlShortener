using System.Security.Cryptography;

namespace UrlShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int CodeLength = 7;

        public string GenerateUniqueCode()
        {
            // через stackalloc выделяем память на стеке, а не в куче просто чтобы не засорять память, при выходе из метода она освободится
            // можно сделать и через обычный масссив char и через обычный random из System.Random, если по простому
            Span<byte> randomBytes = stackalloc byte[CodeLength];
            Span<char> result = stackalloc char[CodeLength];

            //можно сделать и через Random.Shared.NextBytes(randomBytes), но RandomNumberGenerator обеспечивает непредсказумеость и более криптостойкий
            RandomNumberGenerator.Fill(randomBytes);
            

            for (int i = 0; i < CodeLength; i++)
            {
                result[i] = Alphabet[randomBytes[i] % Alphabet.Length];
            }

            return new string(result);
        }
    }
}
