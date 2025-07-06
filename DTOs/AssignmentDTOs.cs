namespace MiniLMS.DTOs;

public class AssignmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MaterialContent { get; set; } = string.Empty;
    public string? MaterialUrl { get; set; }
    public string MaterialType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsActive { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public List<QuizDto> Quizzes { get; set; } = new();
    public AssignmentProgressDto? UserProgress { get; set; }
}

public class QuizDto
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string OptionA { get; set; } = string.Empty;
    public string OptionB { get; set; } = string.Empty;
    public string OptionC { get; set; } = string.Empty;
    public string OptionD { get; set; } = string.Empty;
    public string? CorrectAnswer { get; set; } // Only for managers/admins
    public int Points { get; set; }
    public int OrderIndex { get; set; }
    public string? SelectedAnswer { get; set; } // User's answer
}

public class AssignmentProgressDto
{
    public int Id { get; set; }
    public int AssignmentId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalScore { get; set; }
    public int MaxScore { get; set; }
    public string Status { get; set; } = string.Empty;
    public double ScorePercentage => MaxScore > 0 ? (double)TotalScore / MaxScore * 100 : 0;
    public List<QuizAnswerDto> Answers { get; set; } = new();
}

public class QuizAnswerDto
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string Question { get; set; } = string.Empty;
    public string SelectedAnswer { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public DateTime AnsweredAt { get; set; }
}

public class SubmitQuizDto
{
    public int AssignmentId { get; set; }
    public List<QuizSubmissionDto> Answers { get; set; } = new();
}

public class QuizSubmissionDto
{
    public int QuizId { get; set; }
    public string SelectedAnswer { get; set; } = string.Empty;
}
