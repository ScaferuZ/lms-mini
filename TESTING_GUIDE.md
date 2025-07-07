# 🧪 Testing Guide for Mini LMS

## ❗ Important Fix Applied

I've fixed the build issue you were experiencing. The problem was that the main project (`MiniLMS.csproj`) was trying to compile the test files, but the test dependencies (xUnit, Moq) were only available in the test project.

**What I fixed:**
- Added exclusion rules in `MiniLMS.csproj` to exclude test files from the main build
- Created proper project separation
- Added a test task to VS Code tasks

## 🚀 How to Run the Tests

### Option 1: Using the Custom Script (Recommended)
```bash
./run-tests.sh
```

### Option 2: Using VS Code Tasks
1. Open Command Palette (`Ctrl+Shift+P`)
2. Type "Tasks: Run Task"
3. Select "test"

### Option 3: Manual Commands
```bash
# Build main project (should work now without errors)
dotnet build MiniLMS.csproj

# Build test project
dotnet build MiniLMS.Tests/MiniLMS.Tests.csproj

# Run tests
dotnet test MiniLMS.Tests/MiniLMS.Tests.csproj
```

### Option 4: Using Solution File
```bash
# If you want to build everything together
dotnet build MiniLMS.sln

# Run all tests in solution
dotnet test MiniLMS.sln
```

## 📋 What the Tests Cover

The scoring logic tests include:

1. **✅ Perfect Score Test** - All 5 answers correct → 100 points
2. **❌ Zero Score Test** - All answers incorrect → 0 points  
3. **⚖️ Partial Score Test** - 3 correct answers → 60 points
4. **🔤 Case Sensitivity Test** - "A" = "a" → Works correctly
5. **🎯 Custom Points Test** - Different points per question
6. **🚫 Business Rules Tests** - Prevents double submission
7. **⚠️ Error Handling Tests** - Assignment not started scenarios

## 🎯 Expected Test Results

When you run the tests, you should see:

```
✅ SubmitQuizAsync_AllAnswersCorrect_ShouldGiveFullScore - PASSED
✅ SubmitQuizAsync_AllAnswersIncorrect_ShouldGiveZeroScore - PASSED  
✅ SubmitQuizAsync_MixedAnswers_ShouldGivePartialScore - PASSED
✅ SubmitQuizAsync_CaseInsensitiveAnswers_ShouldHandleCorrectly - PASSED
✅ SubmitQuizAsync_CustomPointValues_ShouldCalculateCorrectly - PASSED
✅ SubmitQuizAsync_AlreadyCompleted_ShouldThrowException - PASSED
✅ SubmitQuizAsync_NotStarted_ShouldThrowException - PASSED

Total tests: 7. Passed: 7. Failed: 0.
```

## 🔧 Troubleshooting

### If you still get build errors:
1. **Clean and rebuild:**
   ```bash
   dotnet clean
   dotnet restore
   dotnet build MiniLMS.csproj
   ```

2. **Check project references:**
   ```bash
   dotnet list MiniLMS.Tests/MiniLMS.Tests.csproj reference
   ```

3. **Restore test packages:**
   ```bash
   cd MiniLMS.Tests
   dotnet restore
   ```

### If tests fail:
1. Check that all dependencies are resolved
2. Verify that the main project builds successfully first
3. Look at the test output for specific error messages

## 📊 Test Architecture

The tests use:
- **xUnit** - Testing framework
- **Moq** - Mocking framework for repositories
- **Isolated testing** - No database or external dependencies
- **Comprehensive scenarios** - Covers all scoring edge cases

## 🎉 Success Criteria

Your tests are successful when:
- ✅ All 7 tests pass
- ✅ No build errors
- ✅ Tests run in under 5 seconds
- ✅ Coverage includes all scoring scenarios from the PRD

## 📝 Adding More Tests

To add more tests, edit `MiniLMS.Tests/Services/AssignmentServiceScoringTests.cs` and add new `[Fact]` methods following the existing pattern.
