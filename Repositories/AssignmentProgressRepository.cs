using Microsoft.EntityFrameworkCore;
using MiniLMS.Data;
using MiniLMS.Models;

namespace MiniLMS.Repositories;

public class AssignmentProgressRepository : IAssignmentProgressRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AssignmentProgress?> GetUserProgressAsync(string userId, int assignmentId)
    {
        return await _context.AssignmentProgresses
            .Include(ap => ap.Assignment)
            .Include(ap => ap.User)
            .Include(ap => ap.QuizAnswers)
                .ThenInclude(qa => qa.Quiz)
            .FirstOrDefaultAsync(ap => ap.UserId == userId && ap.AssignmentId == assignmentId);
    }

    public async Task<AssignmentProgress> CreateProgressAsync(AssignmentProgress progress)
    {
        progress.CreatedAt = DateTime.UtcNow;
        _context.AssignmentProgresses.Add(progress);
        await _context.SaveChangesAsync();
        return progress;
    }

    public async Task<AssignmentProgress> UpdateProgressAsync(AssignmentProgress progress)
    {
        _context.AssignmentProgresses.Update(progress);
        await _context.SaveChangesAsync();
        return progress;
    }

    public async Task<IEnumerable<AssignmentProgress>> GetUserProgressesAsync(string userId)
    {
        return await _context.AssignmentProgresses
            .Include(ap => ap.Assignment)
            .Include(ap => ap.User)
            .Where(ap => ap.UserId == userId)
            .OrderByDescending(ap => ap.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AssignmentProgress>> GetAssignmentProgressesAsync(int assignmentId)
    {
        return await _context.AssignmentProgresses
            .Include(ap => ap.Assignment)
            .Include(ap => ap.User)
            .Where(ap => ap.AssignmentId == assignmentId)
            .OrderByDescending(ap => ap.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AssignmentProgress>> GetSubordinateProgressesAsync(string managerId)
    {
        return await _context.AssignmentProgresses
            .Include(ap => ap.Assignment)
            .Include(ap => ap.User)
            .Where(ap => ap.User.ManagerId == managerId)
            .OrderByDescending(ap => ap.CreatedAt)
            .ToListAsync();
    }
}
