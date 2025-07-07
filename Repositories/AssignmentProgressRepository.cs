using Microsoft.EntityFrameworkCore;
using MiniLMS.Data;
using MiniLMS.Models;

namespace MiniLMS.Repositories;

public class AssignmentProgressRepository : IAssignmentProgressRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public AssignmentProgressRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<AssignmentProgress?> GetUserProgressAsync(string userId, int assignmentId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.AssignmentProgresses
            .Include(ap => ap.Assignment)
            .Include(ap => ap.User)
            .Include(ap => ap.QuizAnswers)
                .ThenInclude(qa => qa.Quiz)
            .FirstOrDefaultAsync(ap => ap.UserId == userId && ap.AssignmentId == assignmentId);
    }

    public async Task<AssignmentProgress> CreateProgressAsync(AssignmentProgress progress)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        progress.CreatedAt = DateTime.UtcNow;
        context.AssignmentProgresses.Add(progress);
        await context.SaveChangesAsync();
        return progress;
    }

    public async Task<AssignmentProgress> UpdateProgressAsync(AssignmentProgress progress)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.AssignmentProgresses.Update(progress);
        await context.SaveChangesAsync();
        return progress;
    }

    public async Task<IEnumerable<AssignmentProgress>> GetUserProgressesAsync(string userId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.AssignmentProgresses
            .Include(ap => ap.Assignment)
            .Include(ap => ap.User)
            .Where(ap => ap.UserId == userId)
            .OrderByDescending(ap => ap.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AssignmentProgress>> GetAssignmentProgressesAsync(int assignmentId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.AssignmentProgresses
            .Include(ap => ap.Assignment)
            .Include(ap => ap.User)
            .Where(ap => ap.AssignmentId == assignmentId)
            .OrderByDescending(ap => ap.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AssignmentProgress>> GetSubordinateProgressesAsync(string managerId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.AssignmentProgresses
            .Include(ap => ap.Assignment)
            .Include(ap => ap.User)
            .Where(ap => ap.User.ManagerId == managerId)
            .OrderByDescending(ap => ap.CreatedAt)
            .ToListAsync();
    }
}
