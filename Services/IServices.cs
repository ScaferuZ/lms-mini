using MiniLMS.DTOs;
using MiniLMS.Models;
using MiniLMS.Data;

namespace MiniLMS.Services;

public interface IAssignmentService
{
    Task<IEnumerable<AssignmentDto>> GetActiveAssignmentsAsync(string userId);
    Task<AssignmentDto?> GetAssignmentDetailsAsync(int id, string userId);
    Task<AssignmentDto> CreateAssignmentAsync(AssignmentDto assignmentDto, string creatorId);
    Task<bool> StartAssignmentAsync(int assignmentId, string userId);
    Task<AssignmentProgressDto> SubmitQuizAsync(SubmitQuizDto submitDto, string userId);
    Task<bool> CanUserAccessAssignmentAsync(int assignmentId, string userId);
    Task<bool> HasUserCompletedAssignmentAsync(int assignmentId, string userId);
}

public interface IProgressService
{
    Task<IEnumerable<AssignmentProgressDto>> GetUserProgressesAsync(string userId);
    Task<IEnumerable<AssignmentProgressDto>> GetSubordinateProgressesAsync(string managerId);
    Task<AssignmentProgressDto?> GetProgressDetailsAsync(int progressId, string userId);
    Task<IEnumerable<AssignmentProgressDto>> GetAssignmentProgressReportAsync(int assignmentId, string managerId);
}

public interface IUserService
{
    Task<ApplicationUser?> GetUserByIdAsync(string userId);
    Task<IEnumerable<ApplicationUser>> GetSubordinatesAsync(string managerId);
    Task<bool> IsManagerAsync(string userId);
    Task<bool> CanUserViewProgressAsync(string managerId, string learnerUserId);
}
