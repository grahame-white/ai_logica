#!/bin/bash

# Check if git hooks are installed
# This script helps developers verify their development environment is properly configured

echo "🔍 Checking git hooks installation status..."

hooks_installed=true

# Check if pre-commit hook exists and is executable
if [ ! -f ".git/hooks/pre-commit" ] || [ ! -x ".git/hooks/pre-commit" ]; then
    echo "❌ Pre-commit hook not installed"
    hooks_installed=false
else
    echo "✅ Pre-commit hook installed"
fi

# Check if pre-push hook exists and is executable
if [ ! -f ".git/hooks/pre-push" ] || [ ! -x ".git/hooks/pre-push" ]; then
    echo "❌ Pre-push hook not installed"
    hooks_installed=false
else
    echo "✅ Pre-push hook installed"
fi

if [ "$hooks_installed" = false ]; then
    echo ""
    echo "⚠️  WARNING: Git hooks are not properly installed!"
    echo ""
    echo "This means formatting issues could slip through and cause CI failures."
    echo "Please install the git hooks immediately:"
    echo ""
    echo "  ./scripts/setup-git-hooks.sh"
    echo ""
    echo "This is especially important for AI developers who should install"
    echo "hooks proactively at the start of any development session."
    exit 1
else
    echo ""
    echo "🎉 All git hooks are properly installed!"
    echo "Your development environment is correctly configured to prevent formatting issues."
fi