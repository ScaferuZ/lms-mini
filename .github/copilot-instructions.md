<!-- Use this file to provide workspace-specific custom instructions to Copilot. For more details, visit https://code.visualstudio.com/docs/copilot/copilot-customization#_use-a-githubcopilotinstructionsmd-file -->

# Mini LMS Project Instructions

This is a .NET 8 Blazor Server application implementing a Mini Learning Management System (LMS) with the following architecture:

## Architecture

- **Clean Architecture**: Controller → Service → Repository pattern
- **Frontend**: Blazor Server with Interactive Server components
- **Backend**: ASP.NET Core Web API controllers
- **Database**: Entity Framework Core with SQLite (configurable for SQL Server/PostgreSQL)
- **Authentication**: ASP.NET Core Identity with cookie-based authentication

## Key Features

- **User Roles**: Learner, Manager, Admin
- **Assignment Management**: Create, view, and manage assignments with learning materials
- **Quiz System**: Multiple-choice questions (5 questions per assignment, 20 points each)
- **Progress Tracking**: Track assignment completion and scores
- **Manager Dashboard**: View subordinate progress and results

## Project Structure

- `Models/`: Domain entities (Assignment, Quiz, AssignmentProgress, QuizAnswer)
- `DTOs/`: Data Transfer Objects for API communication
- `Services/`: Business logic layer
- `Repositories/`: Data access layer
- `Controllers/`: API endpoints
- `Components/Pages/`: Blazor pages for UI
- `Data/`: Entity Framework DbContext and ApplicationUser

## Key Rules

- Users can only submit answers once per assignment
- Scoring: 1 correct answer = 20 points (total 100 points possible)
- Manager can only view subordinate progress
- Clean separation of concerns between layers
- Use dependency injection for all services

## Development Notes

- Use Entity Framework migrations for database changes
- Follow RESTful API patterns for controllers
- Implement proper authorization checks
- Use DTOs for data transfer between layers
- Follow async/await patterns throughout
