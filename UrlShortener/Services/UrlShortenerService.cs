using System.Security.Cryptography;
using System.Text;

namespace UrlShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int CodeLength = 7; 

        public string GenerateUniqueCode()
        {
            // Почему RandomNumberGenerator?
            // System.Random предсказуем, если знать seed (время). 
            // Криптографический генератор гарантирует непредсказуемость ссылки.
            var token = new StringBuilder(CodeLength);
            var bytes = new byte[CodeLength];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            foreach (var b in bytes)
            {
                token.Append(Alphabet[b % Alphabet.Length]);
            }

            return token.ToString();
        }
    }
}
