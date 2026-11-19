using System.ComponentModel.DataAnnotations;

namespace portfolio.Models;

public class Image
{
    public long ImageId { get; set; }
    public long ProjectId { get; set; }

    [Required]
    [StringLength(500)]
    [RegularExpression(@"^https?://.+", ErrorMessage = "Must start with http:// or https://")]
    public string PublicUrl { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Caption { get; set; }
}
