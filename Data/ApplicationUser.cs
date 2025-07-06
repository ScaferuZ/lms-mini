using Microsoft.AspNetCore.Identity;
using MiniLMS.Models;

namespace MiniLMS.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Learner;
    public string? ManagerId { get; set; } // For hierarchical structure
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties
    public ApplicationUser? Manager { get; set; }
    public virtual ICollection<ApplicationUser> Subordinates { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Assignment> CreatedAssignments { get; set; } = new List<Assignment>();
    public virtual ICollection<AssignmentProgress> AssignmentProgresses { get; set; } = new List<AssignmentProgress>();
}

public enum UserRole
{
    Learner,
    Manager,
    Admin
}

