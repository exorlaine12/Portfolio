using System.ComponentModel.DataAnnotations;

namespace portfolio.Models;

public class Project
{
    public long ProjectId { get; set; }

    [Required]
    public long CourseId { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [DataType(DataType.MultilineText)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime? CompletedDate { get; set; }

    [StringLength(500)]
    [RegularExpression(@"^(https?:\/\/)?(www\.)?(youtube\.com\/watch\?v=|youtu\.be\/)[A-Za-z0-9_-]{11}",
        ErrorMessage = "Enter a valid YouTube link")]
    [Display(Name = "YouTube Video URL")]
    public string? VideoUrl { get; set; }

    // Display only
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public List<Image> Images { get; set; } = new();

    // Form input
    [Display(Name = "Image URLs (one per line)")]
    public string? ImageUrls { get; set; }
}
