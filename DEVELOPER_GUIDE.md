# AI Logica - Developer Guide

## Welcome to AI Logica Development

This guide provides information for both human and AI developers contributing to the AI Logica project. The project is currently in early development, focused on building a logic gate simulator using AI-assisted development practices.

## Getting Started

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022, VS Code, or any C# compatible editor
- Git for version control
- Modern web browser for testing

### Quick Start
```bash
# Clone the repository
git clone https://github.com/grahame-white/ai_logica.git
cd ai_logica

# Set up git hooks to prevent formatting issues (recommended)
./scripts/setup-git-hooks.sh

# Build the solution
dotnet build

# Run tests
dotnet test

# Start the application
cd AiLogica
dotnet run
```

The application will be available at `https://localhost:5001` (or the port shown in console output).

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
# Build all projects
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
./scripts/check-format.sh

# Format code and verify it's correct
./scripts/format-code.sh

# Or manually format code
dotnet format
```

#### Automatic Formatting Prevention
To prevent formatting issues from ever being committed or pushed:

```bash
# Install git hooks that check formatting automatically
./scripts/setup-git-hooks.sh
```

This installs:
- **Pre-commit hook**: Checks formatting before each commit and rejects commits with formatting issues
- **Pre-push hook**: Provides a final formatting check before pushing to remote

#### Manual Formatting Check
If git hooks are not installed, always run formatting checks manually:

```bash
# Before committing
./scripts/check-format.sh

# Fix formatting issues
./scripts/format-code.sh
```

#### CI Integration
The CI pipeline includes a formatting verification step that will fail if code is not properly formatted. The git hooks prevent this from happening by catching issues locally.

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test AiLogica.Tests
```

### Running the Application
```bash
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

