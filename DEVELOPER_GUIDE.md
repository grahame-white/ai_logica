# AI Logica - Developer Guide

## Welcome to AI Logica Development

This guide provides information for both human and AI developers contributing to the AI Logica project. The project is currently in early development, focused on building a logic gate simulator using AI-assisted development practices.

## ⚠️ Critical First Step for All Developers

**Before making any commits or changes, you MUST install the git hooks to prevent formatting issues that cause CI failures:**

```bash
script/setup-git-hooks
```

This is especially important for AI developers who should install these hooks proactively at the start of any new conversation thread or development session.

## Getting Started

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022, VS Code, or any C# compatible editor
- Git for version control
- Modern web browser for testing

### Quick Start

#### Using Standard Scripts (Recommended)
```bash
# Clone the repository
git clone https://github.com/grahame-white/ai_logica.git
cd ai_logica

# Complete setup for first-time development
script/setup

# Start the development server
script/server
```

#### Manual Setup (Alternative)
```bash
# Clone the repository
git clone https://github.com/grahame-white/ai_logica.git
cd ai_logica

# Set up git hooks to prevent formatting issues (REQUIRED)
script/setup-git-hooks

# Verify git hooks are installed (optional check)
script/check-git-hooks

# Build the solution
dotnet build

# Run tests
dotnet test

# Start the application
cd AiLogica
dotnet run
```

The application will be available at `https://localhost:5001` (or the port shown in console output).

## Development Standards and Scripts

AI Logica follows GitHub's ["scripts to rule them all"](https://github.com/github/scripts-to-rule-them-all) pattern as a core development standard. This pattern provides a consistent, technology-agnostic interface for common development tasks, ensuring that developers can onboard quickly and workflows remain stable as the technology stack evolves.

### Why "Scripts to Rule Them All"?

- **Consistency**: Same commands work regardless of underlying technology
- **Onboarding**: New developers run `script/setup` and are ready to contribute  
- **CI/CD Alignment**: Local development mirrors production environment
- **Future-Proofing**: Interface remains stable even as tools change

### Core Standardized Scripts

AI Logica implements the following standardized scripts in the `script/` directory:

#### script/setup
Complete setup for first-time development:
```bash
script/setup
```
This script:
- Installs all dependencies
- Sets up git hooks for code formatting
- Builds the application
- Runs tests to verify everything works

#### script/server
Start the development server:
```bash
script/server
```
This starts the Blazor application server for local development.

#### script/test
Run the test suite:
```bash
# Run all tests
script/test

# Run specific test project
script/test AiLogica.Tests
```

#### script/update
Update the application after pulling changes:
```bash
script/update
```
This script:
- Installs any new dependencies
- Rebuilds the application

#### script/bootstrap
Install dependencies only:
```bash
script/bootstrap
```
This script only installs .NET dependencies without building or testing.

#### script/cibuild
Run continuous integration checks:
```bash
script/cibuild
```
This script replicates CI environment checks:
- Installs dependencies
- Checks code formatting
- Builds in Release configuration
- Runs all tests

These scripts provide a consistent interface regardless of the underlying technology stack and are the recommended way to interact with the project.

### Additional Utility Scripts
The `script/` directory also contains utility scripts for specific tasks:

- **`script/setup-git-hooks`** - Install git hooks for automatic formatting checks
- **`script/check-git-hooks`** - Verify git hooks are properly installed  
- **`script/format`** - Format code and check formatting (use `--check` for CI mode)

These utilities are called by the main standardized scripts but can also be used independently when needed.

### Guidelines for Future Script Development

**All future development tools and workflows must align with the "scripts to rule them all" pattern:**

- **New standardized scripts** should be added to the core set only when they represent common, cross-cutting development tasks
- **Technology-specific tools** should be abstracted behind the standardized interface  
- **Script naming** must follow the established conventions (`script/verb` format)
- **Documentation** must be updated to reflect any new scripts or changes to existing ones
- **Backward compatibility** should be maintained unless there's a compelling reason for breaking changes

When adding new development capabilities:
1. **First** determine if it fits an existing standardized script (setup, server, test, etc.)
2. **If new script needed**, follow the pattern naming and ensure it serves a general development need
3. **Add utility scripts** for specific tasks that support the main standardized scripts
4. **Update documentation** in both README.md and this guide
5. **Ensure CI integration** aligns with local development experience

This ensures the development experience remains consistent and discoverable as the project evolves.

## Current Project Structure

For detailed information about the project structure and technical architecture, see [ARCHITECTURE.md](ARCHITECTURE.md).

### Current Implementation
- **ViewModelBase**: Foundation for MVVM pattern implementation
- **Standard Blazor Components**: Template pages (Home, Counter, Weather, Error)
- **Project Structure**: Multi-project solution setup for separation of concerns

## Development Guidelines

### For Human Developers
- Follow standard .NET and Blazor development practices
- Use the existing ViewModelBase for any new view models
- Write tests for new functionality
- Follow the established project structure

### For AI Developers
- **FIRST**: Install git hooks with `script/setup-git-hooks` at the start of every new conversation thread
- Focus on implementing the features outlined in REQUIREMENTS.md
- Build upon the existing MVVM foundation
- Ensure all new code includes appropriate tests
- Follow the architectural patterns established in ARCHITECTURE.md

## Coding Standards

### C# Conventions
- Use PascalCase for public members and types
- Use camelCase for private fields and local variables
- Follow standard .NET naming conventions
- Add XML documentation for public APIs

### Project Conventions
- Namespaces should follow the folder structure
- Use dependency injection for services
- Implement proper disposal patterns where needed
- Follow the established separation between presentation and business logic

### Testing Best Practices
- Use specific element selectors in tests rather than generic ones to avoid ambiguity as the application grows
- Add data attributes (e.g., `data-testid`, `data-gate-type`) to elements that need to be tested for reliable selection
- Extract repeated inline styles to CSS classes for better maintainability
- Ensure tests target the specific functionality being tested rather than relying on implementation details
- Write descriptive test names that clearly indicate what is being tested

## Building and Testing

### Building the Solution
```bash
# Using standard scripts (recommended)
script/update  # Install dependencies and build

# Or manually using dotnet
dotnet build

# Build specific project
dotnet build AiLogica.Core

# Build for release
dotnet build -c Release
```

### Code Formatting

**Important**: Always ensure code is properly formatted before committing to avoid CI failures.

```bash
# Check if formatting is correct (simulates CI check)
script/format --check

# Format code and verify it's correct
script/format

# Or manually format code
dotnet format
```

#### Automatic Formatting Prevention
To prevent formatting issues from ever being committed or pushed:

```bash
# Install git hooks that check formatting automatically
script/setup-git-hooks
```

This installs:
- **Pre-commit hook**: Checks formatting before each commit and rejects commits with formatting issues
- **Pre-push hook**: Provides a final formatting check before pushing to remote

#### Verify Git Hooks Installation
To check if git hooks are properly installed:

```bash
# Verify hooks are installed and configured correctly
script/check-git-hooks
```

This is particularly useful for AI developers to verify their environment is properly configured at the start of a development session.

#### Manual Formatting Check
If git hooks are not installed, always run formatting checks manually:

```bash
# Before committing
script/format --check

# Fix formatting issues
script/format
```

#### CI Integration
The CI pipeline includes a formatting verification step that will fail if code is not properly formatted. The git hooks prevent this from happening by catching issues locally.

### Running Tests
```bash
# Using standard scripts (recommended)
script/test

# Run specific test project
script/test AiLogica.Tests

# Or manually using dotnet
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test AiLogica.Tests
```

### Running the Application
```bash
# Using standard scripts (recommended)
script/server

# Or manually using dotnet
# Development mode
cd AiLogica
dotnet run

# Production mode
dotnet run -c Release
```

## Next Steps for Development

Based on the requirements in REQUIREMENTS.md, the next major development tasks include:

1. **Gate Models**: Implement basic logic gate representations
2. **Canvas Component**: Create interactive drawing area for gate placement
3. **Simulation Engine**: Build logic evaluation and state management
4. **UI Components**: Develop gate palette and property panels
5. **Hierarchical Design**: Implement component abstraction and navigation

