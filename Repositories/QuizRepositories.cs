using Microsoft.EntityFrameworkCore;
using MiniLMS.Data;
using MiniLMS.Models;

namespace MiniLMS.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly ApplicationDbContext _context;

    public QuizRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Quiz>> GetQuizzesByAssignmentAsync(int assignmentId)
    {
        return await _context.Quizzes
            .Where(q => q.AssignmentId == assignmentId)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync();
    }

    public async Task<Quiz?> GetQuizByIdAsync(int id)
    {
        return await _context.Quizzes
            .Include(q => q.Assignment)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Quiz> CreateQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Add(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }

    public async Task<Quiz> UpdateQuizAsync(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
        await _context.SaveChangesAsync();
        return quiz;
    }

    public async Task<bool> DeleteQuizAsync(int id)
    {
        var quiz = await _context.Quizzes.FindAsync(id);
        if (quiz == null) return false;

        _context.Quizzes.Remove(quiz);
        await _context.SaveChangesAsync();
        return true;
    }
}

public class QuizAnswerRepository : IQuizAnswerRepository
{
    private readonly ApplicationDbContext _context;

    public QuizAnswerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<QuizAnswer>> GetAnswersByProgressAsync(int progressId)
    {
        return await _context.QuizAnswers
            .Include(qa => qa.Quiz)
            .Where(qa => qa.AssignmentProgressId == progressId)
            .OrderBy(qa => qa.Quiz.OrderIndex)
            .ToListAsync();
    }

    public async Task<QuizAnswer> CreateAnswerAsync(QuizAnswer answer)
    {
        answer.AnsweredAt = DateTime.UtcNow;
        _context.QuizAnswers.Add(answer);
        await _context.SaveChangesAsync();
        return answer;
    }

    public async Task<bool> HasUserAnsweredQuizAsync(string userId, int assignmentId)
    {
        return await _context.QuizAnswers
            .AnyAsync(qa => qa.AssignmentProgress.UserId == userId && 
                           qa.AssignmentProgress.AssignmentId == assignmentId);
    }
}
