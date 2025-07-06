using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Models;

public class Quiz
{
    public int Id { get; set; }
    
    public int AssignmentId { get; set; }
    
    [Required]
    [MaxLength(500)]
    public string Question { get; set; } = string.Empty;
    
    [Required]
    public string OptionA { get; set; } = string.Empty;
    
    [Required]
    public string OptionB { get; set; } = string.Empty;
    
    [Required]
    public string OptionC { get; set; } = string.Empty;
    
    [Required]
    public string OptionD { get; set; } = string.Empty;
    
    [Required]
    public string CorrectAnswer { get; set; } = string.Empty; // A, B, C, or D
    
    public int Points { get; set; } = 20; // Default 20 points per question
    
    public int OrderIndex { get; set; } // For question ordering
    
    // Navigation properties
    public Assignment Assignment { get; set; } = null!;
    public virtual ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
}
