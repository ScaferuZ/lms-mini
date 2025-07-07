using Microsoft.EntityFrameworkCore;
using MiniLMS.Data;
using MiniLMS.Models;

namespace MiniLMS.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public QuizRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<Quiz>> GetQuizzesByAssignmentAsync(int assignmentId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Quizzes
            .Where(q => q.AssignmentId == assignmentId)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync();
    }

    public async Task<Quiz?> GetQuizByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Quizzes
            .Include(q => q.Assignment)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<Quiz> CreateQuizAsync(Quiz quiz)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Quizzes.Add(quiz);
        await context.SaveChangesAsync();
        return quiz;
    }

    public async Task<Quiz> UpdateQuizAsync(Quiz quiz)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Quizzes.Update(quiz);
        await context.SaveChangesAsync();
        return quiz;
    }

    public async Task<bool> DeleteQuizAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var quiz = await context.Quizzes.FindAsync(id);
        if (quiz == null) return false;

        context.Quizzes.Remove(quiz);
        await context.SaveChangesAsync();
        return true;
    }
}

public class QuizAnswerRepository : IQuizAnswerRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public QuizAnswerRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<QuizAnswer>> GetAnswersByProgressAsync(int progressId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.QuizAnswers
            .Include(qa => qa.Quiz)
            .Where(qa => qa.AssignmentProgressId == progressId)
            .OrderBy(qa => qa.Quiz.OrderIndex)
            .ToListAsync();
    }

    public async Task<QuizAnswer> CreateAnswerAsync(QuizAnswer answer)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        answer.AnsweredAt = DateTime.UtcNow;
        context.QuizAnswers.Add(answer);
        await context.SaveChangesAsync();
        return answer;
    }

    public async Task<bool> HasUserAnsweredQuizAsync(string userId, int assignmentId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.QuizAnswers
            .AnyAsync(qa => qa.AssignmentProgress.UserId == userId && 
                           qa.AssignmentProgress.AssignmentId == assignmentId);
    }
}
