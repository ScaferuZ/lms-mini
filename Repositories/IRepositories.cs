using MiniLMS.Models;

namespace MiniLMS.Repositories;

public interface IAssignmentRepository
{
    Task<IEnumerable<Assignment>> GetActiveAssignmentsAsync();
    Task<Assignment?> GetAssignmentByIdAsync(int id);
    Task<Assignment?> GetAssignmentWithQuizzesAsync(int id);
    Task<Assignment> CreateAssignmentAsync(Assignment assignment);
    Task<Assignment> UpdateAssignmentAsync(Assignment assignment);
    Task<bool> DeleteAssignmentAsync(int id);
    Task<IEnumerable<Assignment>> GetAssignmentsByCreatorAsync(string userId);
}

public interface IAssignmentProgressRepository
{
    Task<AssignmentProgress?> GetUserProgressAsync(string userId, int assignmentId);
    Task<AssignmentProgress> CreateProgressAsync(AssignmentProgress progress);
    Task<AssignmentProgress> UpdateProgressAsync(AssignmentProgress progress);
    Task<IEnumerable<AssignmentProgress>> GetUserProgressesAsync(string userId);
    Task<IEnumerable<AssignmentProgress>> GetAssignmentProgressesAsync(int assignmentId);
    Task<IEnumerable<AssignmentProgress>> GetSubordinateProgressesAsync(string managerId);
}

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>> GetQuizzesByAssignmentAsync(int assignmentId);
    Task<Quiz?> GetQuizByIdAsync(int id);
    Task<Quiz> CreateQuizAsync(Quiz quiz);
    Task<Quiz> UpdateQuizAsync(Quiz quiz);
    Task<bool> DeleteQuizAsync(int id);
}

public interface IQuizAnswerRepository
{
    Task<IEnumerable<QuizAnswer>> GetAnswersByProgressAsync(int progressId);
    Task<QuizAnswer> CreateAnswerAsync(QuizAnswer answer);
    Task<bool> HasUserAnsweredQuizAsync(string userId, int assignmentId);
}
