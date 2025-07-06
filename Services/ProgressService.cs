using Microsoft.AspNetCore.Identity;
using MiniLMS.DTOs;
using MiniLMS.Data;
using MiniLMS.Repositories;

namespace MiniLMS.Services;

public class ProgressService : IProgressService
{
    private readonly IAssignmentProgressRepository _progressRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProgressService(IAssignmentProgressRepository progressRepository, UserManager<ApplicationUser> userManager)
    {
        _progressRepository = progressRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<AssignmentProgressDto>> GetUserProgressesAsync(string userId)
    {
        var progresses = await _progressRepository.GetUserProgressesAsync(userId);
        return progresses.Select(MapToProgressDto);
    }

    public async Task<IEnumerable<AssignmentProgressDto>> GetSubordinateProgressesAsync(string managerId)
    {
        var progresses = await _progressRepository.GetSubordinateProgressesAsync(managerId);
        return progresses.Select(MapToProgressDto);
    }

    public async Task<AssignmentProgressDto?> GetProgressDetailsAsync(int progressId, string userId)
    {
        var progress = await _progressRepository.GetUserProgressAsync(userId, progressId);
        return progress != null ? MapToProgressDto(progress) : null;
    }

    public async Task<IEnumerable<AssignmentProgressDto>> GetAssignmentProgressReportAsync(int assignmentId, string managerId)
    {
        var progresses = await _progressRepository.GetAssignmentProgressesAsync(assignmentId);
        
        // Filter only subordinates
        var subordinateProgresses = progresses.Where(p => p.User.ManagerId == managerId);
        
        return subordinateProgresses.Select(MapToProgressDto);
    }

    private static AssignmentProgressDto MapToProgressDto(Models.AssignmentProgress progress)
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

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<IEnumerable<ApplicationUser>> GetSubordinatesAsync(string managerId)
    {
        var users = _userManager.Users.Where(u => u.ManagerId == managerId);
        return await Task.FromResult(users.ToList());
    }

    public async Task<bool> IsManagerAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user?.Role == UserRole.Manager || user?.Role == UserRole.Admin;
    }

    public async Task<bool> CanUserViewProgressAsync(string managerId, string learnerUserId)
    {
        var learner = await _userManager.FindByIdAsync(learnerUserId);
        var manager = await _userManager.FindByIdAsync(managerId);
        
        if (manager?.Role != UserRole.Manager && manager?.Role != UserRole.Admin)
            return false;
            
        return learner?.ManagerId == managerId || manager?.Role == UserRole.Admin;
    }
}
