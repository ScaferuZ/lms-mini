# Assignment Service Scoring Tests

This test suite covers the scoring logic for the Mini LMS assignment quiz system.

## Test Coverage

### 1. `SubmitQuizAsync_AllAnswersCorrect_ShouldGiveFullScore`
- **Purpose**: Tests that when all 5 quiz answers are correct, the user gets full score (100 points)
- **Expected Result**: Total score = 100 (5 questions × 20 points each)
- **Verifies**: All answers are marked as correct and each earns 20 points

### 2. `SubmitQuizAsync_AllAnswersIncorrect_ShouldGiveZeroScore`
- **Purpose**: Tests that when all quiz answers are incorrect, the user gets zero score
- **Expected Result**: Total score = 0
- **Verifies**: All answers are marked as incorrect and each earns 0 points

### 3. `SubmitQuizAsync_MixedAnswers_ShouldGivePartialScore`
- **Purpose**: Tests partial scoring when some answers are correct and others are incorrect
- **Expected Result**: Total score = 60 (3 correct × 20 points each)
- **Verifies**: Correct answers get 20 points, incorrect answers get 0 points

### 4. `SubmitQuizAsync_CaseInsensitiveAnswers_ShouldHandleCorrectly`
- **Purpose**: Tests that the scoring logic handles case-insensitive answer comparison
- **Expected Result**: Total score = 100 (answers like "a" should match "A")
- **Verifies**: Case-insensitive string comparison works correctly

### 5. `SubmitQuizAsync_CustomPointValues_ShouldCalculateCorrectly`
- **Purpose**: Tests scoring with custom point values per question (not the default 20)
- **Test Data**: Questions worth 10, 15, 20, 25, 30 points respectively
- **Expected Result**: Only correct answers contribute to the total score
- **Verifies**: Different point values per question are handled correctly

### 6. `SubmitQuizAsync_AlreadyCompleted_ShouldThrowException`
- **Purpose**: Tests that attempting to submit answers for an already completed assignment throws an exception
- **Expected Result**: InvalidOperationException with message "Assignment already completed"
- **Verifies**: Business rule that prevents double submission

### 7. `SubmitQuizAsync_NotStarted_ShouldThrowException`
- **Purpose**: Tests that attempting to submit answers for an assignment that hasn't been started throws an exception
- **Expected Result**: InvalidOperationException with message "Assignment not started"
- **Verifies**: Business rule that requires assignment to be started first

## Scoring Logic Overview

The scoring system works as follows:

1. **Quiz Setup**: Each quiz question has a configurable point value (default: 20 points)
2. **Answer Evaluation**: Each submitted answer is compared against the correct answer using case-insensitive string comparison
3. **Point Calculation**: 
   - Correct answer = Full points for that question
   - Incorrect answer = 0 points
4. **Total Score**: Sum of all earned points across all questions
5. **Progress Update**: The assignment progress is updated with the total score and marked as completed

## Key Business Rules Tested

- **Single Submission**: Users can only submit answers once per assignment
- **Start Required**: Assignment must be started before answers can be submitted
- **Case Insensitive**: Answer comparison is case-insensitive (A = a)
- **Flexible Points**: Each question can have different point values
- **All or Nothing**: Each question is either fully correct (full points) or incorrect (0 points)

## Running the Tests

To run these tests:

```bash
dotnet test MiniLMS.Tests/MiniLMS.Tests.csproj
```

To run with detailed output:

```bash
dotnet test MiniLMS.Tests/MiniLMS.Tests.csproj -v n
```

## Test Dependencies

The tests use the following mocking approach:
- **Moq**: For mocking repository dependencies
- **xUnit**: As the testing framework
- **In-Memory**: For any database-related testing needs

All external dependencies (repositories, user manager) are mocked to ensure the tests focus purely on the business logic without external dependencies.
