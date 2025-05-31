#!/bin/bash

# AI Logica - End-to-End Testing Script
# This script sets up and runs end-to-end tests using Playwright

set -e

echo "🎭 AI Logica - End-to-End Testing Setup and Execution"
echo "=================================================="
echo ""

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
echo "📋 Checking prerequisites..."

if ! command_exists dotnet; then
    echo "❌ .NET SDK is not installed. Please install .NET 8 SDK first."
    exit 1
fi

if ! command_exists pwsh; then
    echo "❌ PowerShell is not installed. Please install PowerShell first."
    echo "   Ubuntu/Debian: sudo apt-get install -y powershell"
    echo "   macOS: brew install powershell"
    exit 1
fi

echo "✅ Prerequisites satisfied"
echo ""

# Build the solution
echo "🔨 Building solution..."
dotnet build --configuration Release
echo "✅ Build completed"
echo ""

# Install Playwright browsers
echo "🌐 Installing Playwright browsers..."
PLAYWRIGHT_SCRIPT="./AiLogica.Tests/bin/Release/net8.0/playwright.ps1"

if [ ! -f "$PLAYWRIGHT_SCRIPT" ]; then
    echo "❌ Playwright script not found. Please run 'dotnet build' first."
    exit 1
fi

# Install Playwright browsers
echo "🌐 Installing Playwright browsers..."
PLAYWRIGHT_SCRIPT="./AiLogica.Tests/bin/Release/net8.0/playwright.ps1"

if [ ! -f "$PLAYWRIGHT_SCRIPT" ]; then
    echo "❌ Playwright script not found. Please run 'dotnet build' first."
    exit 1
fi

# Try to install Firefox via Playwright
echo "ℹ️  Attempting to install Firefox via Playwright..."
if pwsh "$PLAYWRIGHT_SCRIPT" install firefox; then
    echo "✅ Playwright Firefox installed successfully"
else
    echo "⚠️  Failed to install Firefox via Playwright (likely due to firewall restrictions)"
    echo "    The E2E tests require Playwright's Firefox browser to be available."
    echo "    In CI environments, this may require adding playwright.azureedge.net to the firewall allow list."
    echo ""
    echo "    For now, you can run the infrastructure tests that don't require a browser:"
    echo "    dotnet test --filter \"EndToEndInfrastructureTests\""
    exit 1
fi
echo "✅ Playwright browsers installed"
echo ""

# Run infrastructure tests first
echo "🏗️ Running end-to-end infrastructure tests..."
dotnet test --no-build --configuration Release --filter "EndToEndInfrastructureTests" --verbosity normal
echo "✅ Infrastructure tests passed"
echo ""

# Run end-to-end tests
echo "🧪 Running end-to-end tests..."
dotnet test --no-build --configuration Release --filter "Category=EndToEnd" --verbosity normal

EXIT_CODE=$?

if [ $EXIT_CODE -eq 0 ]; then
    echo ""
    echo "🎉 All end-to-end tests passed!"
    echo ""
    echo "📊 Test Results Summary:"
    echo "   ✅ Infrastructure tests: Passed"
    echo "   ✅ End-to-end tests: Passed"
    echo ""
    echo "🔍 For AI developers:"
    echo "   - Tests validate complete user workflows"
    echo "   - Screenshots are saved to /tmp/ on failures"
    echo "   - Test output provides structured feedback for debugging"
    echo ""
else
    echo ""
    echo "❌ Some end-to-end tests failed (exit code: $EXIT_CODE)"
    echo ""
    echo "🔧 Troubleshooting:"
    echo "   1. Check the test output above for specific failures"
    echo "   2. Look for screenshot files in /tmp/ for visual debugging"
    echo "   3. Ensure the application is working correctly with unit tests:"
    echo "      dotnet test --filter 'Category!=EndToEnd'"
    echo "   4. Try running the application manually:"
    echo "      cd AiLogica && dotnet run"
    echo ""
fi

exit $EXIT_CODE