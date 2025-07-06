using System.ComponentModel.DataAnnotations;
using MiniLMS.Data;

namespace MiniLMS.Models;

public class AssignmentProgress
{
    public int Id { get; set; }
    
    public int AssignmentId { get; set; }
    
    public string UserId { get; set; } = string.Empty;
    
    public DateTime StartedAt { get; set; }
    
    public DateTime? CompletedAt { get; set; }
    
    public int TotalScore { get; set; } = 0;
    
    public int MaxScore { get; set; } = 100; // 5 questions × 20 points each
    
    public ProgressStatus Status { get; set; } = ProgressStatus.NotStarted;
    
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Assignment Assignment { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public virtual ICollection<QuizAnswer> QuizAnswers { get; set; } = new List<QuizAnswer>();
}

public enum ProgressStatus
{
    NotStarted,
    InProgress,
    Completed,
    Submitted
}
