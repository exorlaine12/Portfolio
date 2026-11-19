using System.ComponentModel.DataAnnotations;

namespace portfolio.Models;

public class Course
{
    public long CourseId { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Use uppercase letters and numbers only")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Semester { get; set; }
    public string? AcademicYear { get; set; }
    public string? Description { get; set; }
}
