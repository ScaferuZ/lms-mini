using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniLMS.Data;
using MiniLMS.Models;

namespace MiniLMS.Services;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DatabaseSeeder(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        // Create sample users if they don't exist
        await CreateSampleUsersAsync();
        
        // Create sample assignments if they don't exist
        await CreateSampleAssignmentsAsync();
        
        // Create sample assignment progress
        await CreateSampleProgressAsync();
    }

    private async Task CreateSampleUsersAsync()
    {
        // Create Admin user
        var adminUser = await _userManager.FindByEmailAsync("admin@minilms.com");
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin@minilms.com",
                Email = "admin@minilms.com",
                FirstName = "Admin",
                LastName = "User",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(adminUser, "Admin123!");
        }

        // Create Manager user with specific ID
        var managerUser = await _userManager.FindByEmailAsync("manager@minilms.com");
        if (managerUser == null)
        {
            managerUser = new ApplicationUser
            {
                Id = "fe271428-fe11-4f33-a754-30be00e32b3e", // Set the specific manager ID
                UserName = "manager@minilms.com",
                Email = "manager@minilms.com",
                FirstName = "Manager",
                LastName = "User",
                Role = UserRole.Manager,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(managerUser, "Manager123!");
        }

        // Create Learner user assigned to the manager
        var learnerUser = await _userManager.FindByEmailAsync("learner@minilms.com");
        if (learnerUser == null)
        {
            learnerUser = new ApplicationUser
            {
                UserName = "learner@minilms.com",
                Email = "learner@minilms.com",
                FirstName = "Learner",
                LastName = "User",
                Role = UserRole.Learner,
                ManagerId = "fe271428-fe11-4f33-a754-30be00e32b3e", // Explicitly set to manager's ID
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(learnerUser, "Learner123!");
        }
        else if (string.IsNullOrEmpty(learnerUser.ManagerId))
        {
            // If learner exists but doesn't have a manager, assign them to the manager
            learnerUser.ManagerId = "fe271428-fe11-4f33-a754-30be00e32b3e";
            await _userManager.UpdateAsync(learnerUser);
        }

        // Create additional learner users for the same manager
        var learnerUser2 = await _userManager.FindByEmailAsync("learner2@minilms.com");
        if (learnerUser2 == null)
        {
            learnerUser2 = new ApplicationUser
            {
                UserName = "learner2@minilms.com",
                Email = "learner2@minilms.com",
                FirstName = "Jane",
                LastName = "Smith",
                Role = UserRole.Learner,
                ManagerId = "fe271428-fe11-4f33-a754-30be00e32b3e", // Same manager
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(learnerUser2, "Learner123!");
        }

        var learnerUser3 = await _userManager.FindByEmailAsync("learner3@minilms.com");
        if (learnerUser3 == null)
        {
            learnerUser3 = new ApplicationUser
            {
                UserName = "learner3@minilms.com",
                Email = "learner3@minilms.com",
                FirstName = "Bob",
                LastName = "Johnson",
                Role = UserRole.Learner,
                ManagerId = "fe271428-fe11-4f33-a754-30be00e32b3e", // Same manager
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };
            await _userManager.CreateAsync(learnerUser3, "Learner123!");
        }
    }

    private async Task CreateSampleAssignmentsAsync()
    {
        if (await _context.Assignments.AnyAsync()) return;

        var adminUser = await _userManager.FindByEmailAsync("admin@minilms.com");
        if (adminUser == null) return;

        var assignments = new List<Assignment>
        {
            new Assignment
            {
                Title = "Introduction to Clean Architecture",
                Description = "Learn the fundamentals of clean architecture and its benefits in software development.",
                MaterialContent = @"
                    <h3>Clean Architecture Overview</h3>
                    <p>Clean Architecture is a software design philosophy that separates concerns to create maintainable, testable, and scalable applications.</p>
                    
                    <h4>Key Principles:</h4>
                    <ul>
                        <li><strong>Dependency Inversion:</strong> High-level modules should not depend on low-level modules. Both should depend on abstractions.</li>
                        <li><strong>Separation of Concerns:</strong> Each layer has a single responsibility.</li>
                        <li><strong>Testability:</strong> Business logic is isolated and easily testable.</li>
                        <li><strong>Independence:</strong> The architecture is independent of frameworks, UI, database, and external services.</li>
                    </ul>
                    
                    <h4>Layers:</h4>
                    <ol>
                        <li><strong>Entities:</strong> Core business logic and rules</li>
                        <li><strong>Use Cases:</strong> Application-specific business rules</li>
                        <li><strong>Interface Adapters:</strong> Convert data between use cases and external systems</li>
                        <li><strong>Frameworks & Drivers:</strong> External systems like databases, web frameworks, etc.</li>
                    </ol>
                    
                    <p>This architecture ensures that your business logic remains pure and unaffected by external changes.</p>
                ",
                MaterialType = MaterialType.Text,
                CreatedByUserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(7),
                IsActive = true
            },
            new Assignment
            {
                Title = "Entity Framework Core Fundamentals",
                Description = "Master the basics of Entity Framework Core for data access in .NET applications.",
                MaterialContent = @"
                    <h3>Entity Framework Core</h3>
                    <p>Entity Framework Core (EF Core) is a modern object-relational mapping (ORM) framework for .NET applications.</p>
                    
                    <h4>Key Features:</h4>
                    <ul>
                        <li><strong>Code First:</strong> Define your model using C# classes</li>
                        <li><strong>Database First:</strong> Generate model from existing database</li>
                        <li><strong>Migrations:</strong> Version control for your database schema</li>
                        <li><strong>LINQ Support:</strong> Query your data using LINQ</li>
                        <li><strong>Change Tracking:</strong> Automatic tracking of entity changes</li>
                    </ul>
                    
                    <h4>DbContext:</h4>
                    <p>The DbContext represents a session with the database and provides APIs for:</p>
                    <ul>
                        <li>Querying data</li>
                        <li>Tracking changes</li>
                        <li>Saving changes</li>
                        <li>Configuring relationships</li>
                    </ul>
                    
                    <h4>Best Practices:</h4>
                    <ol>
                        <li>Use async/await for database operations</li>
                        <li>Implement proper error handling</li>
                        <li>Use repository pattern for testability</li>
                        <li>Configure relationships explicitly</li>
                        <li>Use migrations for schema changes</li>
                    </ol>
                ",
                MaterialType = MaterialType.Text,
                CreatedByUserId = adminUser.Id,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(10),
                IsActive = true
            }
        };

        foreach (var assignment in assignments)
        {
            _context.Assignments.Add(assignment);
        }
        
        await _context.SaveChangesAsync();

        // Add quizzes for the assignments
        await CreateQuizzesForAssignments();
    }

    private async Task CreateQuizzesForAssignments()
    {
        var assignments = await _context.Assignments.ToListAsync();
        
        foreach (var assignment in assignments)
        {
            if (assignment.Title.Contains("Clean Architecture"))
            {
                var quizzes = new List<Quiz>
                {
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "What is the main principle of Clean Architecture?",
                        OptionA = "Dependency Inversion",
                        OptionB = "Code Reusability",
                        OptionC = "Performance Optimization",
                        OptionD = "Database Optimization",
                        CorrectAnswer = "A",
                        Points = 20,
                        OrderIndex = 1
                    },
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "Which layer contains the core business logic?",
                        OptionA = "Interface Adapters",
                        OptionB = "Entities",
                        OptionC = "Frameworks & Drivers",
                        OptionD = "Use Cases",
                        CorrectAnswer = "B",
                        Points = 20,
                        OrderIndex = 2
                    },
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "What makes Clean Architecture testable?",
                        OptionA = "Complex dependencies",
                        OptionB = "Isolated business logic",
                        OptionC = "Database integration",
                        OptionD = "UI coupling",
                        CorrectAnswer = "B",
                        Points = 20,
                        OrderIndex = 3
                    },
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "How many main layers are in Clean Architecture?",
                        OptionA = "3",
                        OptionB = "4",
                        OptionC = "5",
                        OptionD = "6",
                        CorrectAnswer = "B",
                        Points = 20,
                        OrderIndex = 4
                    },
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "What should high-level modules depend on?",
                        OptionA = "Low-level modules",
                        OptionB = "Abstractions",
                        OptionC = "Concrete implementations",
                        OptionD = "External frameworks",
                        CorrectAnswer = "B",
                        Points = 20,
                        OrderIndex = 5
                    }
                };
                
                _context.Quizzes.AddRange(quizzes);
            }
            else if (assignment.Title.Contains("Entity Framework"))
            {
                var quizzes = new List<Quiz>
                {
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "What does EF Core stand for?",
                        OptionA = "Entity Framework Core",
                        OptionB = "Extended Framework Core",
                        OptionC = "Enterprise Framework Core",
                        OptionD = "Efficient Framework Core",
                        CorrectAnswer = "A",
                        Points = 20,
                        OrderIndex = 1
                    },
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "What is DbContext used for?",
                        OptionA = "Configuration only",
                        OptionB = "Database session management",
                        OptionC = "UI rendering",
                        OptionD = "File operations",
                        CorrectAnswer = "B",
                        Points = 20,
                        OrderIndex = 2
                    },
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "What are EF Core migrations used for?",
                        OptionA = "Data backup",
                        OptionB = "Schema version control",
                        OptionC = "Performance tuning",
                        OptionD = "Security management",
                        CorrectAnswer = "B",
                        Points = 20,
                        OrderIndex = 3
                    },
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "Which approach defines model using C# classes?",
                        OptionA = "Database First",
                        OptionB = "Model First",
                        OptionC = "Code First",
                        OptionD = "Schema First",
                        CorrectAnswer = "C",
                        Points = 20,
                        OrderIndex = 4
                    },
                    new Quiz
                    {
                        AssignmentId = assignment.Id,
                        Question = "What is the recommended pattern for database operations?",
                        OptionA = "Synchronous operations",
                        OptionB = "Async/await pattern",
                        OptionC = "Blocking operations",
                        OptionD = "Thread-based operations",
                        CorrectAnswer = "B",
                        Points = 20,
                        OrderIndex = 5
                    }
                };
                
                _context.Quizzes.AddRange(quizzes);
            }
        }
        
        await _context.SaveChangesAsync();
    }

    private async Task CreateSampleProgressAsync()
    {
        // Get users
        var learnerUser = await _userManager.FindByEmailAsync("learner@minilms.com");
        var learnerUser2 = await _userManager.FindByEmailAsync("learner2@minilms.com");
        var learnerUser3 = await _userManager.FindByEmailAsync("learner3@minilms.com");
        
        // Get assignments
        var assignments = await _context.Assignments.ToListAsync();
        
        if (learnerUser != null && assignments.Any())
        {
            // Create progress for learner@minilms.com
            foreach (var assignment in assignments.Take(2)) // Complete first 2 assignments
            {
                var existingProgress = await _context.AssignmentProgresses
                    .FirstOrDefaultAsync(ap => ap.UserId == learnerUser.Id && ap.AssignmentId == assignment.Id);
                
                if (existingProgress == null)
                {
                    var progress = new AssignmentProgress
                    {
                        UserId = learnerUser.Id,
                        AssignmentId = assignment.Id,
                        StartedAt = DateTime.UtcNow.AddDays(-7 + assignment.Id),
                        CompletedAt = DateTime.UtcNow.AddDays(-5 + assignment.Id), // Stagger completion dates
                        TotalScore = 80 + (assignment.Id * 5), // Varying scores
                        Status = ProgressStatus.Completed,
                        CreatedAt = DateTime.UtcNow.AddDays(-7 + assignment.Id),
                        QuizAnswers = new List<QuizAnswer>()
                    };
                    
                    // Get quizzes for this assignment
                    var quizzes = await _context.Quizzes
                        .Where(q => q.AssignmentId == assignment.Id)
                        .OrderBy(q => q.OrderIndex)
                        .ToListAsync();
                    
                    // Create quiz answers
                    foreach (var quiz in quizzes)
                    {
                        var answer = new QuizAnswer
                        {
                            QuizId = quiz.Id,
                            SelectedAnswer = quiz.CorrectAnswer, // All correct for now
                            IsCorrect = true,
                            AnsweredAt = DateTime.UtcNow.AddDays(-5 + assignment.Id),
                            AssignmentProgress = progress
                        };
                        progress.QuizAnswers.Add(answer);
                    }
                    
                    _context.AssignmentProgresses.Add(progress);
                }
            }
        }
        
        if (learnerUser2 != null && assignments.Any())
        {
            // Create progress for learner2@minilms.com
            var assignment = assignments.First();
            var existingProgress = await _context.AssignmentProgresses
                .FirstOrDefaultAsync(ap => ap.UserId == learnerUser2.Id && ap.AssignmentId == assignment.Id);
            
            if (existingProgress == null)
            {
                var progress = new AssignmentProgress
                {
                    UserId = learnerUser2.Id,
                    AssignmentId = assignment.Id,
                    StartedAt = DateTime.UtcNow.AddDays(-5),
                    CompletedAt = DateTime.UtcNow.AddDays(-3),
                    TotalScore = 60, // Lower score
                    Status = ProgressStatus.Completed,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    QuizAnswers = new List<QuizAnswer>()
                };
                
                // Get quizzes for this assignment
                var quizzes = await _context.Quizzes
                    .Where(q => q.AssignmentId == assignment.Id)
                    .OrderBy(q => q.OrderIndex)
                    .ToListAsync();
                
                // Create quiz answers - mix of correct and incorrect
                for (int i = 0; i < quizzes.Count; i++)
                {
                    var quiz = quizzes[i];
                    var isCorrect = i < 3; // First 3 correct, last 2 incorrect
                    var answer = new QuizAnswer
                    {
                        QuizId = quiz.Id,
                        SelectedAnswer = isCorrect ? quiz.CorrectAnswer : "A", // Wrong answer if not correct
                        IsCorrect = isCorrect,
                        AnsweredAt = DateTime.UtcNow.AddDays(-3),
                        AssignmentProgress = progress
                    };
                    progress.QuizAnswers.Add(answer);
                }
                
                _context.AssignmentProgresses.Add(progress);
            }
        }
        
        if (learnerUser3 != null && assignments.Any())
        {
            // Create incomplete progress for learner3@minilms.com
            var assignment = assignments.First();
            var existingProgress = await _context.AssignmentProgresses
                .FirstOrDefaultAsync(ap => ap.UserId == learnerUser3.Id && ap.AssignmentId == assignment.Id);
            
            if (existingProgress == null)
            {
                var progress = new AssignmentProgress
                {
                    UserId = learnerUser3.Id,
                    AssignmentId = assignment.Id,
                    StartedAt = DateTime.UtcNow.AddDays(-2),
                    CompletedAt = null,
                    TotalScore = 0,
                    Status = ProgressStatus.InProgress,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    QuizAnswers = new List<QuizAnswer>()
                };
                
                _context.AssignmentProgresses.Add(progress);
            }
        }
        
        await _context.SaveChangesAsync();
    }
}
