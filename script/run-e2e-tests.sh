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

echo "✅ Prerequisites satisfied"
echo ""

# Build the solution
echo "🔨 Building solution..."
dotnet build --configuration Release
echo "✅ Build completed"
echo ""

# Install system Firefox and Playwright dependencies
echo "🌐 Setting up Firefox for end-to-end testing..."

# Check if Firefox is already installed
if command_exists firefox; then
    echo "✅ System Firefox already installed: $(firefox --version)"
else
    echo "📦 Installing system Firefox..."
    if command_exists apt-get; then
        # Ubuntu/Debian
        sudo apt-get update && sudo apt-get install -y firefox
    elif command_exists dnf; then
        # Fedora
        sudo dnf install -y firefox
    elif command_exists yum; then
        # CentOS/RHEL
        sudo yum install -y firefox
    elif command_exists pacman; then
        # Arch Linux
        sudo pacman -S --noconfirm firefox
    elif command_exists brew; then
        # macOS
        brew install --cask firefox
    else
        echo "⚠️  Cannot automatically install Firefox on this system."
        echo "    Please install Firefox manually and ensure it's in your PATH."
        echo "    Then re-run this script."
        exit 1
    fi
    
    if command_exists firefox; then
        echo "✅ System Firefox installed: $(firefox --version)"
    else
        echo "❌ Firefox installation failed."
        exit 1
    fi
fi

# Setup virtual display for headless testing
echo "🖥️  Setting up virtual display for headless testing..."
if command_exists Xvfb; then
    echo "✅ Xvfb already available"
elif command_exists apt-get; then
    echo "📦 Installing Xvfb..."
    sudo apt-get install -y xvfb
else
    echo "⚠️  Cannot install Xvfb automatically. Tests may fail in headless environments."
fi

# Start virtual display if needed
if [ -z "$DISPLAY" ] && command_exists Xvfb; then
    echo "🖥️  Starting virtual display..."
    export DISPLAY=:99
    Xvfb :99 -screen 0 1280x720x24 &
    sleep 2
    echo "✅ Virtual display started on $DISPLAY"
fi

# Install Playwright dependencies (fallback support)
echo "🎭 Installing Playwright dependencies..."
PLAYWRIGHT_SCRIPT="./AiLogica.Tests/bin/Release/net8.0/playwright.ps1"

if [ ! -f "$PLAYWRIGHT_SCRIPT" ]; then
    echo "❌ Playwright script not found. Please run 'dotnet build' first."
    exit 1
fi

if command_exists pwsh; then
    # Try to install Playwright Firefox as fallback
    echo "ℹ️  Attempting to install Playwright Firefox as fallback..."
    if pwsh "$PLAYWRIGHT_SCRIPT" install firefox; then
        echo "✅ Playwright Firefox installed as fallback"
    else
        echo "⚠️  Playwright Firefox installation failed, but system Firefox should work"
        echo "    (This is expected if network restrictions block downloads)"
    fi
else
    echo "⚠️  PowerShell not found. Using system Firefox only."
fi
echo ""

# Run infrastructure tests first
echo "🏗️ Running end-to-end infrastructure tests..."
dotnet test --no-build --configuration Release --filter "EndToEndInfrastructureTests" --verbosity normal
echo "✅ Infrastructure tests passed"
echo ""

# Run end-to-end tests
echo "🧪 Running end-to-end tests..."
if [ -n "$DISPLAY" ]; then
    echo "ℹ️  Using display: $DISPLAY"
fi

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