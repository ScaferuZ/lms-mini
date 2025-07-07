#!/bin/bash

echo "🧪 Running Mini LMS Scoring Logic Tests..."
echo "========================================="

# Change to project directory
cd "$(dirname "$0")"

# Build the main project first (excluding tests)
echo "📦 Building main project..."
dotnet build MiniLMS.csproj

if [ $? -ne 0 ]; then
    echo "❌ Main project build failed!"
    exit 1
fi

echo "✅ Main project built successfully!"
echo ""

# Build the test project
echo "📦 Building test project..."
dotnet build MiniLMS.Tests/MiniLMS.Tests.csproj

if [ $? -ne 0 ]; then
    echo "❌ Test project build failed!"
    exit 1
fi

echo "✅ Test project built successfully!"
echo ""

# Run the tests
echo "🧪 Running scoring logic tests..."
dotnet test MiniLMS.Tests/MiniLMS.Tests.csproj --verbosity normal

echo ""
echo "✅ Test execution completed!"
