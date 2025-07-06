using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MiniLMS.Models;

namespace MiniLMS.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Assignment> Assignments { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<AssignmentProgress> AssignmentProgresses { get; set; }
    public DbSet<QuizAnswer> QuizAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Assignment relationships
        builder.Entity<Assignment>()
            .HasOne(a => a.CreatedBy)
            .WithMany(u => u.CreatedAssignments)
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Quiz relationships
        builder.Entity<Quiz>()
            .HasOne(q => q.Assignment)
            .WithMany(a => a.Quizzes)
            .HasForeignKey(q => q.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure AssignmentProgress relationships
        builder.Entity<AssignmentProgress>()
            .HasOne(ap => ap.Assignment)
            .WithMany(a => a.AssignmentProgresses)
            .HasForeignKey(ap => ap.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AssignmentProgress>()
            .HasOne(ap => ap.User)
            .WithMany(u => u.AssignmentProgresses)
            .HasForeignKey(ap => ap.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure QuizAnswer relationships
        builder.Entity<QuizAnswer>()
            .HasOne(qa => qa.Quiz)
            .WithMany(q => q.QuizAnswers)
            .HasForeignKey(qa => qa.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<QuizAnswer>()
            .HasOne(qa => qa.AssignmentProgress)
            .WithMany(ap => ap.QuizAnswers)
            .HasForeignKey(qa => qa.AssignmentProgressId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure ApplicationUser self-referencing relationship
        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Manager)
            .WithMany(u => u.Subordinates)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure unique constraint for one submission per user per assignment
        builder.Entity<AssignmentProgress>()
            .HasIndex(ap => new { ap.UserId, ap.AssignmentId })
            .IsUnique();

        // Configure enum conversions
        builder.Entity<Assignment>()
            .Property(a => a.MaterialType)
            .HasConversion<string>();

        builder.Entity<AssignmentProgress>()
            .Property(ap => ap.Status)
            .HasConversion<string>();

        builder.Entity<ApplicationUser>()
            .Property(u => u.Role)
            .HasConversion<string>();
    }
}
