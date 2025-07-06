using MiniLMS.DTOs;
using MiniLMS.Models;
using MiniLMS.Repositories;

namespace MiniLMS.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IAssignmentRepository _assignmentRepository;
    private readonly IAssignmentProgressRepository _progressRepository;
    private readonly IQuizRepository _quizRepository;
    private readonly IQuizAnswerRepository _answerRepository;

    public AssignmentService(
        IAssignmentRepository assignmentRepository,
        IAssignmentProgressRepository progressRepository,
        IQuizRepository quizRepository,
        IQuizAnswerRepository answerRepository)
    {
        _assignmentRepository = assignmentRepository;
        _progressRepository = progressRepository;
        _quizRepository = quizRepository;
        _answerRepository = answerRepository;
    }

    public async Task<IEnumerable<AssignmentDto>> GetActiveAssignmentsAsync(string userId)
    {
        var assignments = await _assignmentRepository.GetActiveAssignmentsAsync();
        var assignmentDtos = new List<AssignmentDto>();

        foreach (var assignment in assignments)
        {
            var progress = await _progressRepository.GetUserProgressAsync(userId, assignment.Id);
            
            assignmentDtos.Add(new AssignmentDto
            {
                Id = assignment.Id,
                Title = assignment.Title,
                Description = assignment.Description,
                MaterialContent = assignment.MaterialContent,
                MaterialUrl = assignment.MaterialUrl,
                MaterialType = assignment.MaterialType.ToString(),
                CreatedAt = assignment.CreatedAt,
                DueDate = assignment.DueDate,
                IsActive = assignment.IsActive,
                CreatedByUserName = assignment.CreatedBy.UserName ?? string.Empty,
                UserProgress = progress != null ? MapToProgressDto(progress) : null
            });
        }

        return assignmentDtos;
    }

    public async Task<AssignmentDto?> GetAssignmentDetailsAsync(int id, string userId)
    {
        var assignment = await _assignmentRepository.GetAssignmentWithQuizzesAsync(id);
        if (assignment == null) return null;

        var progress = await _progressRepository.GetUserProgressAsync(userId, id);
        var quizDtos = new List<QuizDto>();

        foreach (var quiz in assignment.Quizzes)
        {
            var quizDto = new QuizDto
            {
                Id = quiz.Id,
                Question = quiz.Question,
                OptionA = quiz.OptionA,
                OptionB = quiz.OptionB,
                OptionC = quiz.OptionC,
                OptionD = quiz.OptionD,
                Points = quiz.Points,
                OrderIndex = quiz.OrderIndex
            };

            // If user has answered, show their selected answer
            if (progress != null)
            {
                var userAnswer = progress.QuizAnswers.FirstOrDefault(qa => qa.QuizId == quiz.Id);
                if (userAnswer != null)
                {
                    quizDto.SelectedAnswer = userAnswer.SelectedAnswer;
                }
            }

            quizDtos.Add(quizDto);
        }

        return new AssignmentDto
        {
            Id = assignment.Id,
            Title = assignment.Title,
            Description = assignment.Description,
            MaterialContent = assignment.MaterialContent,
            MaterialUrl = assignment.MaterialUrl,
            MaterialType = assignment.MaterialType.ToString(),
            CreatedAt = assignment.CreatedAt,
            DueDate = assignment.DueDate,
            IsActive = assignment.IsActive,
            CreatedByUserName = assignment.CreatedBy.UserName ?? string.Empty,
            Quizzes = quizDtos,
            UserProgress = progress != null ? MapToProgressDto(progress) : null
        };
    }

    public async Task<AssignmentDto> CreateAssignmentAsync(AssignmentDto assignmentDto, string creatorId)
    {
        var assignment = new Assignment
        {
            Title = assignmentDto.Title,
            Description = assignmentDto.Description,
            MaterialContent = assignmentDto.MaterialContent,
            MaterialUrl = assignmentDto.MaterialUrl,
            MaterialType = Enum.Parse<MaterialType>(assignmentDto.MaterialType),
            DueDate = assignmentDto.DueDate,
            CreatedByUserId = creatorId,
            IsActive = true
        };

        var createdAssignment = await _assignmentRepository.CreateAssignmentAsync(assignment);
        
        // Create quizzes
        foreach (var quizDto in assignmentDto.Quizzes)
        {
            var quiz = new Quiz
            {
                AssignmentId = createdAssignment.Id,
                Question = quizDto.Question,
                OptionA = quizDto.OptionA,
                OptionB = quizDto.OptionB,
                OptionC = quizDto.OptionC,
                OptionD = quizDto.OptionD,
                CorrectAnswer = quizDto.CorrectAnswer ?? string.Empty,
                Points = quizDto.Points,
                OrderIndex = quizDto.OrderIndex
            };

            await _quizRepository.CreateQuizAsync(quiz);
        }

        return await GetAssignmentDetailsAsync(createdAssignment.Id, creatorId) ?? assignmentDto;
    }

    public async Task<bool> StartAssignmentAsync(int assignmentId, string userId)
    {
        var existingProgress = await _progressRepository.GetUserProgressAsync(userId, assignmentId);
        if (existingProgress != null) return false; // Already started

        var progress = new AssignmentProgress
        {
            AssignmentId = assignmentId,
            UserId = userId,
            StartedAt = DateTime.UtcNow,
            Status = ProgressStatus.InProgress
        };

        await _progressRepository.CreateProgressAsync(progress);
        return true;
    }

    public async Task<AssignmentProgressDto> SubmitQuizAsync(SubmitQuizDto submitDto, string userId)
    {
        var progress = await _progressRepository.GetUserProgressAsync(userId, submitDto.AssignmentId);
        if (progress == null)
        {
            throw new InvalidOperationException("Assignment not started");
        }

        if (progress.Status == ProgressStatus.Completed || progress.Status == ProgressStatus.Submitted)
        {
            throw new InvalidOperationException("Assignment already completed");
        }

        var quizzes = await _quizRepository.GetQuizzesByAssignmentAsync(submitDto.AssignmentId);
        var totalScore = 0;

        foreach (var submission in submitDto.Answers)
        {
            var quiz = quizzes.FirstOrDefault(q => q.Id == submission.QuizId);
            if (quiz == null) continue;

            var isCorrect = quiz.CorrectAnswer.Equals(submission.SelectedAnswer, StringComparison.OrdinalIgnoreCase);
            var pointsEarned = isCorrect ? quiz.Points : 0;
            totalScore += pointsEarned;

            var answer = new QuizAnswer
            {
                QuizId = submission.QuizId,
                AssignmentProgressId = progress.Id,
                SelectedAnswer = submission.SelectedAnswer,
                IsCorrect = isCorrect,
                PointsEarned = pointsEarned
            };

            await _answerRepository.CreateAnswerAsync(answer);
        }

        progress.TotalScore = totalScore;
        progress.CompletedAt = DateTime.UtcNow;
        progress.Status = ProgressStatus.Completed;
        await _progressRepository.UpdateProgressAsync(progress);

        return MapToProgressDto(progress);
    }

    public async Task<bool> CanUserAccessAssignmentAsync(int assignmentId, string userId)
    {
        var assignment = await _assignmentRepository.GetAssignmentByIdAsync(assignmentId);
        return assignment?.IsActive == true;
    }

    public async Task<bool> HasUserCompletedAssignmentAsync(int assignmentId, string userId)
    {
        var progress = await _progressRepository.GetUserProgressAsync(userId, assignmentId);
        return progress?.Status == ProgressStatus.Completed || progress?.Status == ProgressStatus.Submitted;
    }

    private static AssignmentProgressDto MapToProgressDto(AssignmentProgress progress)
    {
        return new AssignmentProgressDto
        {
            Id = progress.Id,
            AssignmentId = progress.AssignmentId,
            UserId = progress.UserId,
            UserName = progress.User.UserName ?? string.Empty,
            UserFullName = $"{progress.User.FirstName} {progress.User.LastName}".Trim(),
            StartedAt = progress.StartedAt,
            CompletedAt = progress.CompletedAt,
            TotalScore = progress.TotalScore,
            MaxScore = progress.MaxScore,
            Status = progress.Status.ToString(),
            Answers = progress.QuizAnswers.Select(qa => new QuizAnswerDto
            {
                Id = qa.Id,
                QuizId = qa.QuizId,
                Question = qa.Quiz.Question,
                SelectedAnswer = qa.SelectedAnswer,
                CorrectAnswer = qa.Quiz.CorrectAnswer,
                IsCorrect = qa.IsCorrect,
                PointsEarned = qa.PointsEarned,
                AnsweredAt = qa.AnsweredAt
            }).ToList()
        };
    }
}
