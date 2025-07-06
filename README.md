# Mini LMS - Assignment & Knowledge Check Module

A .NET 8 Blazor Server application implementing a mini Learning Management System (LMS) with clean architecture, featuring assignment management, quiz functionality, and progress tracking.

## Features

### 🎯 Core Functionality

- **Assignment Management**: Create and manage learning assignments with materials
- **Quiz System**: 5 multiple-choice questions per assignment (20 points each)
- **Progress Tracking**: Track completion status and scores
- **User Roles**: Learner, Manager, and Admin roles with appropriate permissions
- **Manager Dashboard**: View subordinate progress and results

### 🏗️ Architecture

- **Clean Architecture**: Controller → Service → Repository pattern
- **Frontend**: Blazor Server with Interactive Server components
- **Backend**: ASP.NET Core Web API controllers
- **Database**: Entity Framework Core with SQLite (configurable for SQL Server/PostgreSQL)
- **Authentication**: ASP.NET Core Identity with cookie-based authentication

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 / VS Code
- SQLite (included) or SQL Server/PostgreSQL (optional)

### Installation

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd mini-lms
   ```

2. **Restore packages**

   ```bash
   dotnet restore
   ```

3. **Update database**

   ```bash
   dotnet ef database update
   ```

4. **Run the application**

   ```bash
   dotnet run
   ```

5. **Access the application**
   - Open your browser to `https://localhost:5001` or `http://localhost:5000`
   - Register a new account or login with existing credentials

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

## 🔧 Configuration

### Database Providers

The application supports multiple database providers:

#### SQLite (Default)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=Data/app.db"
  }
}
```

#### SQL Server

```json
{
  "ConnectionStrings": {
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
