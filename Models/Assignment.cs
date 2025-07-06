using System.ComponentModel.DataAnnotations;
using MiniLMS.Data;

namespace MiniLMS.Models;

public class Assignment
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string MaterialContent { get; set; } = string.Empty;
    
    public string? MaterialUrl { get; set; }
    
    public MaterialType MaterialType { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? DueDate { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public string CreatedByUserId { get; set; } = string.Empty;
    
    // Navigation properties
    public ApplicationUser CreatedBy { get; set; } = null!;
    public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public virtual ICollection<AssignmentProgress> AssignmentProgresses { get; set; } = new List<AssignmentProgress>();
}

public enum MaterialType
{
    Text,
    Pdf,
    Video,
    Link
}
