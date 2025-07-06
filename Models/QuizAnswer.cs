using System.ComponentModel.DataAnnotations;

namespace MiniLMS.Models;

public class QuizAnswer
{
    public int Id { get; set; }
    
    public int QuizId { get; set; }
    
    public int AssignmentProgressId { get; set; }
    
    [Required]
    public string SelectedAnswer { get; set; } = string.Empty; // A, B, C, or D
    
    public bool IsCorrect { get; set; }
    
    public int PointsEarned { get; set; }
    
    public DateTime AnsweredAt { get; set; }
    
    // Navigation properties
    public Quiz Quiz { get; set; } = null!;
    public AssignmentProgress AssignmentProgress { get; set; } = null!;
}
