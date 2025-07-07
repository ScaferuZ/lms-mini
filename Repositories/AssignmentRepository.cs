using Microsoft.EntityFrameworkCore;
using MiniLMS.Data;
using MiniLMS.Models;

namespace MiniLMS.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

    public AssignmentRepository(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<Assignment>> GetActiveAssignmentsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Assignments
            .Where(a => a.IsActive)
            .Include(a => a.CreatedBy)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Assignment?> GetAssignmentByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Assignments
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assignment?> GetAssignmentWithQuizzesAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Assignments
            .Include(a => a.CreatedBy)
            .Include(a => a.Quizzes.OrderBy(q => q.OrderIndex))
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assignment> CreateAssignmentAsync(Assignment assignment)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        assignment.CreatedAt = DateTime.UtcNow;
        context.Assignments.Add(assignment);
        await context.SaveChangesAsync();
        return assignment;
    }

    public async Task<Assignment> UpdateAssignmentAsync(Assignment assignment)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Assignments.Update(assignment);
        await context.SaveChangesAsync();
        return assignment;
    }

    public async Task<bool> DeleteAssignmentAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var assignment = await context.Assignments.FindAsync(id);
        if (assignment == null) return false;

        context.Assignments.Remove(assignment);
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentsByCreatorAsync(string userId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Assignments
            .Where(a => a.CreatedByUserId == userId)
            .Include(a => a.CreatedBy)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}
