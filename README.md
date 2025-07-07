# Mini LMS - Assignment & Knowledge Check Module

A .NET 8 Blazor Server application implementing a mini Learning Management System (LMS) with clean architecture, featuring assignment management, quiz functionality, and progress tracking.

## 🎯 Features

### Core Functionality

- **Assignment Management**: Create and manage learning assignments with materials
- **Quiz System**: 5 multiple-choice questions per assignment (20 points each)
- **Progress Tracking**: Track completion status and scores
- **User Roles**: Learner, Manager, and Admin roles with appropriate permissions
- **Manager Dashboard**: View subordinate progress and results

### Architecture

- **Clean Architecture**: Controller → Service → Repository pattern
- **Frontend**: Blazor Server with Interactive Server components
- **Backend**: ASP.NET Core Web API controllers
- **Database**: Entity Framework Core with SQL Server
- **Authentication**: ASP.NET Core Identity with cookie-based authentication
- **Thread Safety**: IDbContextFactory pattern for Blazor Server concurrency

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK
- Docker (for SQL Server)
- Visual Studio 2022 / VS Code

### 1. Setup SQL Server with Docker

Run SQL Server in a Docker container:

```bash
# Pull and run SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Admin123!" \
   -p 1433:1433 --name sqlserver --hostname sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# Verify container is running
docker ps
```

Alternative using Docker Compose (create `docker-compose.yml`):

```yaml
version: '3.8'
services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=Admin123!
    ports:
      - "1433:1433"
    volumes:
      - sqldata:/var/opt/mssql
volumes:
  sqldata:
```

Then run:
```bash
docker-compose up -d
```

### 2. Clone and Setup Project

```bash
# Clone the repository
git clone <repository-url>
cd mini-lms

# Restore packages
dotnet restore

# Trust development certificates
dotnet dev-certs https --trust
```

### 3. Database Setup

The application uses SQL Server with the following connection string (already configured):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MiniLMS;User ID=sa;Password=Admin123!;Encrypt=True;TrustServerCertificate=True"
  }
}
```

Apply migrations and seed data:

```bash
# Apply database migrations
dotnet ef database update

# Run the application (migrations and seeding happen automatically)
dotnet run
```

### 4. Access the Application

- **URL**: `https://localhost:5123` or `http://localhost:5000`
- **Default Users** (created automatically by DatabaseSeeder):

| Role | Email | Password | Full Name | Manager |
|------|-------|----------|-----------|---------|
| Admin | admin@minilms.com | Admin123! | Admin User | - |
| Manager | manager@minilms.com | Manager123! | Manager User | - |
| Learner | learner@minilms.com | Learner123! | Learner User | Manager User |
| Learner | learner2@minilms.com | Learner123! | Jane Smith | Manager User |
| Learner | learner3@minilms.com | Learner123! | Bob Johnson | Manager User |

**Note**: All learners are assigned to the same manager for testing the manager dashboard functionality.

## 📋 Usage

### For Learners

1. **Login**: Use learner credentials to access the system
2. **View Assignments**: Navigate to home page or `/assignments` to see available assignments
3. **Start Assignment**: Click on an assignment to view materials and start the quiz
4. **Complete Quiz**: Answer all 5 questions and submit to finish the assignment
5. **View Progress**: Check your progress and scores in the `/progress` section

### For Managers

1. **Login**: Use manager credentials to access management features
2. **Navigate to Progress**: Access the **Progress** section to view team member progress
3. **Create Assignments**: Use the API endpoints to create new assignments for your team
4. **Monitor Performance**: Track completion rates, scores, and learning outcomes of subordinates
5. **View Reports**: Access detailed progress reports for your subordinates

### For Admins

1. **Login**: Use admin credentials for full system access
2. **System Overview**: Access both assignments and progress sections
3. **View All Data**: View system-wide progress and results regardless of hierarchy
4. **Manage System**: Manage all assignments, users, and system configuration
5. **Full Control**: Create, edit, and manage all aspects of the LMS

## 🏗️ Project Structure

```
MiniLMS/
├── Components/
│   ├── Pages/              # Blazor pages
│   │   ├── Home.razor      # Landing page with role-based content
│   │   ├── Assignments.razor # Assignment list for learners
│   │   ├── AssignmentDetails.razor # Assignment details and quiz
│   │   └── Progress.razor   # Progress tracking for managers
│   └── Layout/             # Layout components
├── Controllers/            # API controllers
├── Data/                   # DbContext and ApplicationUser
├── DTOs/                   # Data Transfer Objects
├── Models/                 # Domain entities
├── Repositories/           # Data access layer
├── Services/               # Business logic layer
├── Migrations/             # EF Core migrations
└── wwwroot/               # Static files
```

## 🔧 Development Scripts

### Essential Commands

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Run with specific URLs
dotnet run --urls="https://localhost:5001;http://localhost:5000"

# Watch for changes (hot reload)
dotnet watch run

# Run tests
dotnet test MiniLMS.Tests/MiniLMS.Tests.csproj

# Or use the test script
./run-tests.sh
```

### Database Management

```bash
# Create a new migration
dotnet ef migrations add <MigrationName>

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove

# Drop database
dotnet ef database drop

# Generate SQL script
dotnet ef migrations script
```

**🌱 Database Seeding:**
- Sample data is automatically created on application startup
- DatabaseSeeder runs after migrations are applied
- Creates users, assignments, and sample progress data
- Uses consistent IDs for testing (Manager ID: `fe271428-fe11-4f33-a754-30be00e32b3e`)
- All accounts have `EmailConfirmed = true` for immediate access

### Docker Commands

```bash
# Start SQL Server container
docker start sqlserver

# Stop SQL Server container
docker stop sqlserver

# Remove SQL Server container
docker rm sqlserver

# View container logs
docker logs sqlserver

# Connect to SQL Server container
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Admin123!
```

## 📊 Database Schema

### Core Entities

- **ApplicationUser**: Extended Identity user with role and manager hierarchy
- **Assignment**: Learning assignments with materials and metadata
- **Quiz**: Multiple-choice questions for each assignment
- **AssignmentProgress**: User progress tracking for assignments
- **QuizAnswer**: User answers to quiz questions

### Key Relationships

- User → Manager (hierarchical relationship)
- Assignment → CreatedBy (User)
- Assignment → Quizzes (1:Many)
- AssignmentProgress → User & Assignment
- QuizAnswer → Quiz & AssignmentProgress

## �️ Technical Details

### Architecture Patterns

- **Repository Pattern**: Data access abstraction
- **Service Layer**: Business logic separation
- **DTO Pattern**: Data transfer between layers
- **Clean Architecture**: Dependency inversion and separation of concerns

### Key Technologies

- **.NET 8**: Latest framework with performance improvements
- **Blazor Server**: Interactive web UI with SignalR
- **Entity Framework Core**: ORM with Code First approach
- **ASP.NET Core Identity**: Authentication and authorization
- **AutoMapper**: Object-to-object mapping
- **SQL Server**: Production-ready database

### Performance Optimizations

- **IDbContextFactory**: Thread-safe DbContext creation for Blazor Server
- **Async/Await**: Non-blocking operations throughout
- **Connection Pooling**: Efficient database connection management
- **Lazy Loading**: Optimized data fetching

## 🔒 Security Features

- **Authentication**: Cookie-based authentication via ASP.NET Core Identity
- **Authorization**: Role-based access control
- **Input Validation**: Form validation and sanitization
- **SQL Injection Prevention**: EF Core parameterized queries
- **CSRF Protection**: Anti-forgery tokens

## 🧪 Testing

### Unit Tests

The project includes comprehensive unit tests for the scoring logic and business rules.

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project (correct path)
dotnet test MiniLMS.Tests/MiniLMS.Tests.csproj

# Run tests with detailed output
dotnet test MiniLMS.Tests/MiniLMS.Tests.csproj --verbosity normal

# Run specific test class
dotnet test --filter "AssignmentServiceScoringTests"

# Run using the custom test script
./run-tests.sh
```

### Test Coverage

**Scoring Logic Tests** (7 comprehensive tests):
- ✅ Perfect score (100 points for 5/5 correct answers)
- ✅ Zero score (0 points for all incorrect answers)
- ✅ Partial score (60 points for 3/5 correct answers)
- ✅ Case-insensitive answer comparison ("A" = "a")
- ✅ Custom point values per question
- ✅ Business rule validation (prevents double submission)
- ✅ Error handling (assignment not started scenarios)

**Key Testing Rules:**
- Each question worth 20 points by default (configurable)
- Case-insensitive answer matching
- Single submission per assignment
- Total possible score: 100 points (5 questions × 20 points)

### Using VS Code Tasks

You can also run tests using VS Code:
1. Press `Ctrl+Shift+P`
2. Type "Tasks: Run Task"
3. Select "test"

### Manual Testing

1. **Assignment Flow**: Create → View → Complete Quiz → Check Progress
2. **Manager Dashboard**: Login as manager → View team progress
3. **Role Permissions**: Verify access controls for different roles
4. **Concurrent Users**: Test multiple users simultaneously

##  Deployment

### Local Development

```bash
# Development mode
dotnet run --environment Development

# Production simulation
dotnet run --environment Production
```

### Production Deployment

1. **Database**: Update connection string for production SQL Server
2. **Secrets**: Use Azure Key Vault or environment variables
3. **Hosting**: Deploy to Azure App Service or IIS
4. **SSL**: Configure HTTPS certificates

## 📚 Learning Resources

### Business Rules

- Users can only submit answers **once per assignment**
- Scoring: 1 correct answer = 20 points (total 100 points possible)
- Managers can only view **subordinate progress**
- Clean separation of concerns between layers

### Sample Data

The application automatically creates comprehensive sample data on first run:

**👥 User Hierarchy:**
```
Admin User (admin@minilms.com)
└── Manager User (manager@minilms.com)
    ├── Learner User (learner@minilms.com)
    ├── Jane Smith (learner2@minilms.com)  
    └── Bob Johnson (learner3@minilms.com)
```

**📚 Sample Assignments:**
1. **"Introduction to Clean Architecture"** 
   - Learn software design principles and dependency inversion
   - Quiz covers: main principles, layer structure, testability concepts
   
2. **"Entity Framework Core Fundamentals"** 
   - Master ORM concepts and database operations
   - Quiz covers: DbContext, Code First, migrations, LINQ querying
   
3. **"Blazor Server Development"** 
   - Build interactive web applications with real-time features
   - Quiz covers: components, lifecycle, SignalR, state management

Each assignment includes:
- Rich HTML learning materials with structured content
- 5 multiple-choice questions (A, B, C, D options)
- 20 points per question (100 points total)
- Due dates set 7-10 days from creation
- Questions directly related to the learning material

**📊 Sample Progress Data:**
- Some learners have completed assignments (for testing manager dashboard)
- Various completion statuses and scores
- Realistic progress tracking scenarios

**🎯 Testing Scenarios:**
- **Manager View**: Login as manager to see all 3 subordinates' progress
- **Learner Experience**: Complete assignments and see score calculation
- **Admin Overview**: Access to all users and assignments across the system

---

**Mini LMS** - A clean, scalable learning management system built with modern .NET technologies.
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MiniLMS;Trusted_Connection=true"
  }
}
```

#### PostgreSQL

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=MiniLMS;Username=postgres;Password=your_password"
  }
}
```

### Authentication

The application uses ASP.NET Core Identity with cookie-based authentication. Email confirmation is required by default but can be disabled in development.

## 🛠️ Development

### Adding New Features

1. Create domain models in `Models/`
2. Add DTOs in `DTOs/`
3. Implement repositories in `Repositories/`
4. Add business logic in `Services/`
5. Create API controllers in `Controllers/`
6. Build Blazor pages in `Components/Pages/`

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add [MigrationName]

# Update database
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

## 📋 Test Architecture

### Current Test Implementation

- **Framework**: xUnit with Moq for mocking
- **Coverage**: Scoring logic business rules
- **Isolation**: No external dependencies (database, file system)
- **Performance**: Fast execution (< 5 seconds)

### Test Organization

```
MiniLMS.Tests/
├── Services/
│   └── AssignmentServiceScoringTests.cs  # Scoring logic tests
├── GlobalUsings.cs                       # Common using statements
├── SimpleTest.cs                         # Basic verification test
└── README.md                            # Detailed test documentation
```

### Future Test Expansion

- Repository tests for data access patterns
- Controller tests for API endpoint validation
- Integration tests for end-to-end workflows
- Authentication and authorization tests

## 📝 API Endpoints

### Assignments

- `GET /api/assignments` - Get active assignments
- `GET /api/assignments/{id}` - Get assignment details
- `POST /api/assignments` - Create new assignment (Manager/Admin)
- `POST /api/assignments/{id}/start` - Start assignment
- `POST /api/assignments/{id}/submit` - Submit quiz answers

### Progress

- `GET /api/progress/my-progress` - Get user's progress
- `GET /api/progress/subordinates` - Get team progress (Manager/Admin)
- `GET /api/progress/assignment/{id}/report` - Get assignment report (Manager/Admin)

## 🔐 Security

### Authentication

- Cookie-based authentication using ASP.NET Core Identity
- Role-based authorization (Learner, Manager, Admin)
- Secure password requirements and validation

### Authorization

- Route-level authorization attributes
- Service-level permission checks
- Manager can only view subordinate data
- Admin has full access to all data

## 📋 Business Rules

### Assignment Rules

- Users can only submit quiz answers once per assignment
- Assignments must have exactly 5 questions
- Each question is worth 20 points (total 100 points)
- Users must view material before accessing quiz

### Scoring Rules

- Correct answer = 20 points
- Incorrect answer = 0 points
- Maximum score = 100 points
- Minimum passing score = 60 points (configurable)

### Manager Rules

- Managers can only view subordinate progress
- Managers can create assignments for their team
- Hierarchical relationship: Manager → Subordinates

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Implement your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For issues and questions:

1. Check the documentation
2. Review existing issues
3. Create a new issue with detailed description
4. Provide steps to reproduce any bugs

## 🎯 Success Criteria

✅ **Functional Requirements**

- User can view and complete assignments
- Quiz scoring works correctly
- Progress tracking is accurate
- Manager can view subordinate progress
- Role-based access control functions

✅ **Technical Requirements**

- Clean architecture implementation
- Proper separation of concerns
- Entity Framework migrations work
- Authentication and authorization
- Responsive Blazor UI

✅ **Quality Requirements**

- Code is maintainable and testable
- Database structure is normalized
- Error handling is implemented
- Security best practices followed
- Documentation is comprehensive
