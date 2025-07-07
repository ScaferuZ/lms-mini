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

- **URL**: `https://localhost:5001` or `http://localhost:5000`
- **Default Users** (created automatically):

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@minilms.com | Admin123! |
| Manager | manager@minilms.com | Manager123! |
| Learner | learner@minilms.com | Learner123! |
| Learner | learner2@minilms.com | Learner123! |
| Learner | learner3@minilms.com | Learner123! |

## 📋 Usage

### For Learners

1. Login with learner credentials
2. View available assignments on the home page
3. Click on an assignment to view materials and take quiz
4. Complete the 5-question quiz to finish the assignment
5. View progress and scores

### For Managers

1. Login with manager credentials
2. Navigate to **Progress** section
3. View team members' assignment progress
4. Monitor completion rates and scores
5. Track learning outcomes for subordinates

### For Admins

1. Login with admin credentials
2. Access both assignments and progress sections
3. View system-wide progress and results
4. Manage assignments and users

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
dotnet test
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

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Tests/MiniLMS.Tests.csproj
```

### Manual Testing

1. **Assignment Flow**: Create → View → Complete Quiz → Check Progress
2. **Manager Dashboard**: Login as manager → View team progress
3. **Role Permissions**: Verify access controls for different roles
4. **Concurrent Users**: Test multiple users simultaneously

## 📝 API Documentation

### Available Endpoints

- `GET /api/assignments` - Get active assignments
- `GET /api/assignments/{id}` - Get assignment details
- `POST /api/assignments/{id}/submit` - Submit quiz answers
- `GET /api/progress/user/{userId}` - Get user progress
- `GET /api/progress/team/{managerId}` - Get team progress

## 🚀 Deployment

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

The application automatically creates sample data including:
- 3 assignments with 5 questions each
- Multiple users with different roles
- Sample progress data showing completed assignments

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For issues and questions:
- Create an issue in the repository
- Check the documentation
- Review the code comments and examples

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

## 🎮 Usage

### For Learners

1. **View Assignments**: Navigate to `/assignments` to see available assignments
2. **Start Assignment**: Click on an assignment to view materials and start the quiz
3. **Complete Quiz**: Answer all 5 questions and submit
4. **View Progress**: Check your progress and scores in the `/progress` section

### For Managers

1. **Create Assignments**: Use the API endpoints to create new assignments
2. **View Reports**: Access the Progress section to view team member progress
3. **Monitor Performance**: Track completion rates and scores of subordinates

### For Admins

- Full access to all features
- Can view all user progress regardless of hierarchy
- Can manage all assignments and users

## 🛠️ Development

### Project Structure

```
MiniLMS/
├── Components/
│   ├── Pages/              # Blazor pages
│   ├── Layout/             # Layout components
│   └── Account/            # Identity components
├── Controllers/            # API controllers
├── Data/                   # Entity Framework context
├── DTOs/                   # Data Transfer Objects
├── Models/                 # Domain entities
├── Repositories/           # Data access layer
├── Services/              # Business logic layer
└── wwwroot/               # Static files
```

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

## 🧪 Testing

### Unit Tests

- Service layer tests for business logic
- Repository tests for data access
- Controller tests for API endpoints

### Integration Tests

- End-to-end testing of complete workflows
- Database integration tests
- Authentication flow tests

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

## 🚀 Deployment

### Development

```bash
dotnet run --environment Development
```

### Production

```bash
dotnet publish -c Release
```

### Docker (Optional)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY bin/Release/net8.0/publish/ App/
WORKDIR /App
EXPOSE 80
ENTRYPOINT ["dotnet", "MiniLMS.dll"]
```

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
