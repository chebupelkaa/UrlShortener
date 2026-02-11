using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Models
{
    public class ShortLink
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите URL")]
        [Url(ErrorMessage = "Некорректный формат URL")]
        [MaxLength(2048)]
        public string OriginalUrl { get; set; } = string.Empty;

        public string ShortCode { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public long ClickCount { get; set; }
    }
}
