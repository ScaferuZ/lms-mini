#!/bin/bash

echo "🔍 Verifying test setup..."

# Check if test project builds
echo "📦 Building test project..."
cd /home/scaf/Documents/code/lms-mini
dotnet build MiniLMS.Tests/MiniLMS.Tests.csproj --no-restore --verbosity quiet

if [ $? -eq 0 ]; then
    echo "✅ Test project builds successfully!"
    
    echo "🧪 Running tests..."
    dotnet test MiniLMS.Tests/MiniLMS.Tests.csproj --no-build --verbosity minimal
    
    if [ $? -eq 0 ]; then
        echo "🎉 All tests passed!"
    else
        echo "❌ Some tests failed - check output above"
    fi
else
    echo "❌ Test project build failed!"
    exit 1
fi
