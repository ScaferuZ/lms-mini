using Microsoft.EntityFrameworkCore;
using MiniLMS.Data;
using MiniLMS.Models;

namespace MiniLMS.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly ApplicationDbContext _context;

    public AssignmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Assignment>> GetActiveAssignmentsAsync()
    {
        return await _context.Assignments
            .Where(a => a.IsActive)
            .Include(a => a.CreatedBy)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Assignment?> GetAssignmentByIdAsync(int id)
    {
        return await _context.Assignments
            .Include(a => a.CreatedBy)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assignment?> GetAssignmentWithQuizzesAsync(int id)
    {
        return await _context.Assignments
            .Include(a => a.CreatedBy)
            .Include(a => a.Quizzes.OrderBy(q => q.OrderIndex))
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assignment> CreateAssignmentAsync(Assignment assignment)
    {
        assignment.CreatedAt = DateTime.UtcNow;
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<Assignment> UpdateAssignmentAsync(Assignment assignment)
    {
        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync();
        return assignment;
    }

    public async Task<bool> DeleteAssignmentAsync(int id)
    {
        var assignment = await _context.Assignments.FindAsync(id);
        if (assignment == null) return false;

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Assignment>> GetAssignmentsByCreatorAsync(string userId)
    {
        return await _context.Assignments
            .Where(a => a.CreatedByUserId == userId)
            .Include(a => a.CreatedBy)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}
