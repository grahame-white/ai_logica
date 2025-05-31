#!/bin/bash

# Format and verify code before committing
# This script formats the code and then verifies no changes are needed

echo "🔧 Formatting code..."
dotnet format

if [ $? -ne 0 ]; then
    echo "❌ dotnet format failed!"
    exit 1
fi

echo "✅ Code formatted successfully!"

echo ""
echo "🔍 Verifying formatting..."
dotnet format --verify-no-changes --verbosity minimal

if [ $? -ne 0 ]; then
    echo "❌ Code still has formatting issues after running dotnet format!"
    echo "This may indicate a problem with the formatting configuration."
    exit 1
fi

echo "✅ Code formatting verification passed!"
echo ""
echo "🚀 Your code is ready to commit!"