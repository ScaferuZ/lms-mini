using Moq;
using MiniLMS.Services;
using MiniLMS.Repositories;
using MiniLMS.Models;
using MiniLMS.DTOs;
using Microsoft.AspNetCore.Identity;
using MiniLMS.Data;

namespace MiniLMS.Tests.Services;

public class AssignmentServiceScoringTests
{
    private readonly Mock<IAssignmentRepository> _assignmentRepositoryMock;
    private readonly Mock<IAssignmentProgressRepository> _progressRepositoryMock;
    private readonly Mock<IQuizRepository> _quizRepositoryMock;
    private readonly Mock<IQuizAnswerRepository> _answerRepositoryMock;
    private readonly AssignmentService _assignmentService;

    public AssignmentServiceScoringTests()
    {
        _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
        _progressRepositoryMock = new Mock<IAssignmentProgressRepository>();
        _quizRepositoryMock = new Mock<IQuizRepository>();
        _answerRepositoryMock = new Mock<IQuizAnswerRepository>();

        _assignmentService = new AssignmentService(
            _assignmentRepositoryMock.Object,
            _progressRepositoryMock.Object,
            _quizRepositoryMock.Object,
            _answerRepositoryMock.Object
        );
    }

    [Fact]
    public async Task SubmitQuizAsync_AllAnswersCorrect_ShouldGiveFullScore()
    {
        // Arrange
        var userId = "test-user-id";
        var assignmentId = 1;
        var expectedTotalScore = 100; // 5 questions × 20 points each

        var progress = CreateTestProgress(userId, assignmentId);
        var quizzes = CreateTestQuizzes(assignmentId);
        var submitDto = CreateSubmitDto(assignmentId, allCorrect: true);

        SetupMocks(progress, quizzes);

        // Act
        var result = await _assignmentService.SubmitQuizAsync(submitDto, userId);

        // Assert
        Assert.Equal(expectedTotalScore, result.TotalScore);
        Assert.Equal(ProgressStatus.Completed.ToString(), result.Status);
        Assert.NotNull(result.CompletedAt);
        
        // Verify all answers were created with correct points
        _answerRepositoryMock.Verify(x => x.CreateAnswerAsync(It.Is<QuizAnswer>(qa => 
            qa.IsCorrect == true && qa.PointsEarned == 20)), Times.Exactly(5));
    }

    [Fact]
    public async Task SubmitQuizAsync_AllAnswersIncorrect_ShouldGiveZeroScore()
    {
        // Arrange
        var userId = "test-user-id";
        var assignmentId = 1;
        var expectedTotalScore = 0; // All answers incorrect

        var progress = CreateTestProgress(userId, assignmentId);
        var quizzes = CreateTestQuizzes(assignmentId);
        var submitDto = CreateSubmitDto(assignmentId, allCorrect: false);

        SetupMocks(progress, quizzes);

        // Act
        var result = await _assignmentService.SubmitQuizAsync(submitDto, userId);

        // Assert
        Assert.Equal(expectedTotalScore, result.TotalScore);
        Assert.Equal(ProgressStatus.Completed.ToString(), result.Status);
        
        // Verify all answers were created with zero points
        _answerRepositoryMock.Verify(x => x.CreateAnswerAsync(It.Is<QuizAnswer>(qa => 
            qa.IsCorrect == false && qa.PointsEarned == 0)), Times.Exactly(5));
    }

    [Fact]
    public async Task SubmitQuizAsync_MixedAnswers_ShouldGivePartialScore()
    {
        // Arrange
        var userId = "test-user-id";
        var assignmentId = 1;
        var expectedTotalScore = 60; // 3 correct × 20 points each

        var progress = CreateTestProgress(userId, assignmentId);
        var quizzes = CreateTestQuizzes(assignmentId);
        var submitDto = CreateMixedSubmitDto(assignmentId); // 3 correct, 2 incorrect

        SetupMocks(progress, quizzes);

        // Act
        var result = await _assignmentService.SubmitQuizAsync(submitDto, userId);

        // Assert
        Assert.Equal(expectedTotalScore, result.TotalScore);
        Assert.Equal(ProgressStatus.Completed.ToString(), result.Status);
        
        // Verify correct answers got points and incorrect ones got zero
        _answerRepositoryMock.Verify(x => x.CreateAnswerAsync(It.Is<QuizAnswer>(qa => 
            qa.IsCorrect == true && qa.PointsEarned == 20)), Times.Exactly(3));
        _answerRepositoryMock.Verify(x => x.CreateAnswerAsync(It.Is<QuizAnswer>(qa => 
            qa.IsCorrect == false && qa.PointsEarned == 0)), Times.Exactly(2));
    }

    [Fact]
    public async Task SubmitQuizAsync_CaseInsensitiveAnswers_ShouldHandleCorrectly()
    {
        // Arrange
        var userId = "test-user-id";
        var assignmentId = 1;
        var expectedTotalScore = 100; // All correct with different cases

        var progress = CreateTestProgress(userId, assignmentId);
        var quizzes = CreateTestQuizzes(assignmentId);
        var submitDto = CreateCaseInsensitiveSubmitDto(assignmentId); // Same answers but different cases

        SetupMocks(progress, quizzes);

        // Act
        var result = await _assignmentService.SubmitQuizAsync(submitDto, userId);

        // Assert
        Assert.Equal(expectedTotalScore, result.TotalScore);
        
        // Verify case-insensitive comparison worked
        _answerRepositoryMock.Verify(x => x.CreateAnswerAsync(It.Is<QuizAnswer>(qa => 
            qa.IsCorrect == true && qa.PointsEarned == 20)), Times.Exactly(5));
    }

    [Fact]
    public async Task SubmitQuizAsync_CustomPointValues_ShouldCalculateCorrectly()
    {
        // Arrange
        var userId = "test-user-id";
        var assignmentId = 1;
        var expectedTotalScore = 75; // Custom point values: 10+15+20+25+30 = 100, but only 3 correct

        var progress = CreateTestProgress(userId, assignmentId);
        var quizzes = CreateTestQuizzesWithCustomPoints(assignmentId);
        var submitDto = CreateCustomPointsSubmitDto(assignmentId); // 3 correct answers

        SetupMocks(progress, quizzes);

        // Act
        var result = await _assignmentService.SubmitQuizAsync(submitDto, userId);

        // Assert
        Assert.Equal(expectedTotalScore, result.TotalScore); // 10+20+30+15 = 75 (4 correct out of 5)
    }

    [Fact]
    public async Task SubmitQuizAsync_AlreadyCompleted_ShouldThrowException()
    {
        // Arrange
        var userId = "test-user-id";
        var assignmentId = 1;

        var progress = CreateTestProgress(userId, assignmentId);
        progress.Status = ProgressStatus.Completed; // Already completed

        var submitDto = CreateSubmitDto(assignmentId, allCorrect: true);

        _progressRepositoryMock.Setup(x => x.GetUserProgressAsync(userId, assignmentId))
            .ReturnsAsync(progress);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _assignmentService.SubmitQuizAsync(submitDto, userId));
        
        Assert.Equal("Assignment already completed", exception.Message);
    }

    [Fact]
    public async Task SubmitQuizAsync_NotStarted_ShouldThrowException()
    {
        // Arrange
        var userId = "test-user-id";
        var assignmentId = 1;

        var submitDto = CreateSubmitDto(assignmentId, allCorrect: true);

        _progressRepositoryMock.Setup(x => x.GetUserProgressAsync(userId, assignmentId))
            .ReturnsAsync((AssignmentProgress?)null); // Not started

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _assignmentService.SubmitQuizAsync(submitDto, userId));
        
        Assert.Equal("Assignment not started", exception.Message);
    }

    #region Helper Methods

    private AssignmentProgress CreateTestProgress(string userId, int assignmentId)
    {
        return new AssignmentProgress
        {
            Id = 1,
            UserId = userId,
            AssignmentId = assignmentId,
            StartedAt = DateTime.UtcNow.AddHours(-1),
            Status = ProgressStatus.InProgress,
            QuizAnswers = new List<QuizAnswer>(),
            User = new ApplicationUser 
            { 
                Id = userId, 
                UserName = "testuser", 
                FirstName = "Test", 
                LastName = "User" 
            }
        };
    }

    private List<Quiz> CreateTestQuizzes(int assignmentId)
    {
        return new List<Quiz>
        {
            new Quiz { Id = 1, AssignmentId = assignmentId, Question = "Question 1", CorrectAnswer = "A", Points = 20 },
            new Quiz { Id = 2, AssignmentId = assignmentId, Question = "Question 2", CorrectAnswer = "B", Points = 20 },
            new Quiz { Id = 3, AssignmentId = assignmentId, Question = "Question 3", CorrectAnswer = "C", Points = 20 },
            new Quiz { Id = 4, AssignmentId = assignmentId, Question = "Question 4", CorrectAnswer = "D", Points = 20 },
            new Quiz { Id = 5, AssignmentId = assignmentId, Question = "Question 5", CorrectAnswer = "A", Points = 20 }
        };
    }

    private List<Quiz> CreateTestQuizzesWithCustomPoints(int assignmentId)
    {
        return new List<Quiz>
        {
            new Quiz { Id = 1, AssignmentId = assignmentId, Question = "Question 1", CorrectAnswer = "A", Points = 10 },
            new Quiz { Id = 2, AssignmentId = assignmentId, Question = "Question 2", CorrectAnswer = "B", Points = 15 },
            new Quiz { Id = 3, AssignmentId = assignmentId, Question = "Question 3", CorrectAnswer = "C", Points = 20 },
            new Quiz { Id = 4, AssignmentId = assignmentId, Question = "Question 4", CorrectAnswer = "D", Points = 25 },
            new Quiz { Id = 5, AssignmentId = assignmentId, Question = "Question 5", CorrectAnswer = "A", Points = 30 }
        };
    }

    private SubmitQuizDto CreateSubmitDto(int assignmentId, bool allCorrect)
    {
        return new SubmitQuizDto
        {
            AssignmentId = assignmentId,
            Answers = new List<QuizSubmissionDto>
            {
                new QuizSubmissionDto { QuizId = 1, SelectedAnswer = allCorrect ? "A" : "B" },
                new QuizSubmissionDto { QuizId = 2, SelectedAnswer = allCorrect ? "B" : "A" },
                new QuizSubmissionDto { QuizId = 3, SelectedAnswer = allCorrect ? "C" : "A" },
                new QuizSubmissionDto { QuizId = 4, SelectedAnswer = allCorrect ? "D" : "A" },
                new QuizSubmissionDto { QuizId = 5, SelectedAnswer = allCorrect ? "A" : "B" }
            }
        };
    }

    private SubmitQuizDto CreateMixedSubmitDto(int assignmentId)
    {
        return new SubmitQuizDto
        {
            AssignmentId = assignmentId,
            Answers = new List<QuizSubmissionDto>
            {
                new QuizSubmissionDto { QuizId = 1, SelectedAnswer = "A" }, // Correct
                new QuizSubmissionDto { QuizId = 2, SelectedAnswer = "B" }, // Correct
                new QuizSubmissionDto { QuizId = 3, SelectedAnswer = "C" }, // Correct
                new QuizSubmissionDto { QuizId = 4, SelectedAnswer = "A" }, // Incorrect (should be D)
                new QuizSubmissionDto { QuizId = 5, SelectedAnswer = "B" }  // Incorrect (should be A)
            }
        };
    }

    private SubmitQuizDto CreateCaseInsensitiveSubmitDto(int assignmentId)
    {
        return new SubmitQuizDto
        {
            AssignmentId = assignmentId,
            Answers = new List<QuizSubmissionDto>
            {
                new QuizSubmissionDto { QuizId = 1, SelectedAnswer = "a" }, // Correct (lowercase)
                new QuizSubmissionDto { QuizId = 2, SelectedAnswer = "b" }, // Correct (lowercase)
                new QuizSubmissionDto { QuizId = 3, SelectedAnswer = "c" }, // Correct (lowercase)
                new QuizSubmissionDto { QuizId = 4, SelectedAnswer = "d" }, // Correct (lowercase)
                new QuizSubmissionDto { QuizId = 5, SelectedAnswer = "A" }  // Correct (uppercase)
            }
        };
    }

    private SubmitQuizDto CreateCustomPointsSubmitDto(int assignmentId)
    {
        return new SubmitQuizDto
        {
            AssignmentId = assignmentId,
            Answers = new List<QuizSubmissionDto>
            {
                new QuizSubmissionDto { QuizId = 1, SelectedAnswer = "A" }, // Correct - 10 points
                new QuizSubmissionDto { QuizId = 2, SelectedAnswer = "A" }, // Incorrect - 0 points (should be B)
                new QuizSubmissionDto { QuizId = 3, SelectedAnswer = "C" }, // Correct - 20 points
                new QuizSubmissionDto { QuizId = 4, SelectedAnswer = "D" }, // Correct - 25 points
                new QuizSubmissionDto { QuizId = 5, SelectedAnswer = "A" }  // Correct - 30 points
            }
        };
    }

    private void SetupMocks(AssignmentProgress progress, List<Quiz> quizzes)
    {
        _progressRepositoryMock.Setup(x => x.GetUserProgressAsync(progress.UserId, progress.AssignmentId))
            .ReturnsAsync(progress);

        _quizRepositoryMock.Setup(x => x.GetQuizzesByAssignmentAsync(progress.AssignmentId))
            .ReturnsAsync(quizzes);

        _answerRepositoryMock.Setup(x => x.CreateAnswerAsync(It.IsAny<QuizAnswer>()))
            .ReturnsAsync((QuizAnswer qa) => qa);

        _progressRepositoryMock.Setup(x => x.UpdateProgressAsync(It.IsAny<AssignmentProgress>()))
            .ReturnsAsync((AssignmentProgress ap) => ap);
    }

    #endregion
}
