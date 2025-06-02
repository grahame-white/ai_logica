# AI Logica - Developer Guide

## Welcome to AI Logica Development

This guide provides information for both human and AI developers contributing to the AI Logica project. The project is currently in early development, focused on building a logic gate simulator using AI-assisted development practices.

**Important**: All developers should refer to [GLOSSARY.md](GLOSSARY.md) for consistent terminology when working on issues, writing documentation, or implementing features.

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

### General Guidelines for All Developers
- **Terminology**: Always refer to [GLOSSARY.md](GLOSSARY.md) for consistent terminology when working on issues, writing documentation, or implementing features
- **Documentation**: Update the glossary when introducing new terms or when misunderstandings arise
- **Communication**: Use glossary definitions in issue discussions, code comments, and documentation

### For Human Developers
- Follow standard .NET and Blazor development practices
- Use the existing ViewModelBase for any new ViewModels
- Write tests for new functionality
- Follow the established project structure

### For AI Developers
- **FIRST**: Install git hooks with `script/setup-git-hooks` at the start of every new conversation thread
- **Reference glossary**: Use [GLOSSARY.md](GLOSSARY.md) for consistent terminology throughout development
- Focus on implementing the features outlined in REQUIREMENTS.md
- Build upon the existing MVVM foundation
- Ensure all new code includes appropriate tests
- Follow the architectural patterns established in ARCHITECTURE.md

## AI Developer Incremental Development Guide

### ⚠️ Critical: Prevent Lost Work Through Incremental Development

**Problem**: AI developers often attempt to complete entire features in 1-2 commits, which can consume the entire context window before task completion, resulting in lost uncommitted work.

**Solution**: Adopt an incremental development approach with frequent commits and progress reporting.

### 🎯 Incremental Development Principles

#### 1. **Early and Frequent Commits**
- **Commit every meaningful milestone**, no matter how small
- **Never let more than 10 minutes pass** without committing progress
- **Use `report_progress` tool frequently** to push changes and update the PR

#### 2. **Break Down Large Tasks**
- **Identify 3-5 smaller sub-tasks** from any feature request
- **Implement one sub-task completely** before moving to the next
- **Test and commit each sub-task** independently

#### 3. **Incremental Implementation Pattern**
```bash
# For each sub-task:
1. Plan the specific change (use think tool if needed)
2. Make the minimal change needed
3. Run: script/test (verify no regressions)
4. Run: script/format (ensure formatting)
5. Commit with report_progress tool
6. Move to next sub-task
```

### 📋 Recommended Commit Cadence

#### **After Setup (First Commit)**
```bash
# Always commit after initial setup
script/setup-git-hooks
script/setup
# Use report_progress to commit setup completion
```

#### **During Development (Every 10 minutes)**
- ✅ Added new test case
- ✅ Implemented basic method stub
- ✅ Added validation logic
- ✅ Updated UI component
- ✅ Fixed failing test
- ✅ Added error handling

#### **Before Major Changes**
- ✅ Before refactoring existing code
- ✅ Before changing multiple files
- ✅ Before implementing complex logic

### 🛡️ Context Window Management

#### **Warning Signs You're Doing Too Much**
- Making changes to more than 3-4 files simultaneously
- Adding more than 50 lines of code before committing
- Spending more than 10 minutes without a successful test run
- Implementing multiple requirements in one commit

#### **Recovery Actions**
- **Stop and commit** what works immediately
- **Use report_progress** to save current state
- **Break remaining work** into smaller pieces
- **Continue with incremental approach**

### 🔄 Recommended Workflow for Large Features

#### **Phase 1: Planning and Setup (First commit)**
1. Install git hooks: `script/setup-git-hooks`
2. Run setup: `script/setup`
3. Create initial plan with sub-tasks
4. **Commit**: "Setup environment and create implementation plan"

#### **Phase 2: Test Infrastructure (Second commit)**
1. Add failing tests for the first sub-task
2. Ensure tests compile and fail as expected
3. **Commit**: "Add failing tests for [sub-task]"

#### **Phase 3: Incremental Implementation (Multiple commits)**
1. Implement minimal code to make one test pass
2. Run `script/test` and `script/format`
3. **Commit**: "Implement [specific functionality]"
4. Repeat for each test/requirement

#### **Phase 4: Integration and Polish (Final commits)**
1. Verify all tests pass
2. Test manually with `script/server`
3. **Commit**: "Complete [feature] implementation"

### 📊 Progress Reporting Best Practices

#### **Use report_progress Tool Frequently**
- **After setup** (initial commit)
- **After each sub-task completion**
- **After fixing any test failures**
- **Before making major architectural changes**

#### **Progress Update Content**
- ✅ Completed items in checklist format
- 🚧 Current work in progress
- 📋 Remaining tasks with clear next steps
- 🔧 Any technical decisions or blockers

### 🚨 Emergency Procedures

#### **If Context Window Is Getting Full**
1. **IMMEDIATELY**: Use `report_progress` to commit current state
2. **Document**: What works and what doesn't in the commit message
3. **Prioritize**: Save the most important working changes first
4. **Plan**: Create clear next steps for the next session

#### **If You Must Make Large Changes**
1. **Commit current working state first**
2. **Use feature branches** for experimental changes
3. **Make incremental commits** even within the large change
4. **Test frequently** with `script/test`

### ✅ Success Metrics

**You're following incremental development correctly if:**
- You commit working code every 10 minutes
- Each commit has a single, clear purpose
- Tests pass after every commit
- You never lose more than 10 minutes of work
- You use `report_progress` at least 3-4 times per session
- You can stop work at any commit and have a working system

## Coding Standards

### C# Conventions
- Use PascalCase for public members and types
- Use camelCase for private fields and local variables
- Follow standard .NET naming conventions
- Add XML documentation for public APIs
- Each file shall include at most one class, interface or enum
- Use file level namespaces
- Source files must be logically organised

### Static Analysis and Code Quality
**All violations from warning level and above must be addressed.** The project is configured to treat warnings as errors to enforce code quality standards.

#### Defect-Driven Rule Enforcement Policy
**If a defect is identified that a currently suppressed static analysis rule could have caught, then that rule must be enabled and enforced.** This policy ensures that our static analysis configuration evolves and improves based on real-world defects rather than remaining static.

When implementing this policy:
1. **Investigate defects thoroughly** - Determine if any currently suppressed rule would have detected the issue
2. **Enable the rule** - Remove the rule from `NoWarn` in `Directory.Build.props` or change from `Action="None"` to `Action="Warning"` or `Action="Error"` in `jetbrains-aligned.ruleset`
3. **Fix all violations** - Address all instances of the newly enabled rule across the codebase
4. **Document the change** - Update commit messages and code reviews to explain why the rule was enabled
5. **Prevent regression** - Ensure the rule remains enabled to prevent similar defects in the future

This approach prioritizes practical code quality improvements over theoretical completeness, ensuring our static analysis rules provide genuine value in preventing real issues.

Static analysis is automatically enabled for all projects via `Directory.Build.props` and includes:

#### Core Analysis Engines
- **Microsoft .NET Analyzers**: Built-in code quality and security analysis
- **StyleCop Analyzers**: Code style enforcement with JetBrains-aligned configuration
- **SonarAnalyzer**: Advanced code quality rules complementing JetBrains analysis
- **JetBrains Annotations**: Enhanced code contracts and nullability analysis

#### JetBrains Integration
The static analysis configuration is specifically aligned with JetBrains ReSharper/Rider analysis standards:
- **Field Naming**: Allows underscore prefixes (e.g., `_field`) following JetBrains conventions
- **Using Directives**: Supports both inside and outside namespace placement
- **Code Style**: Flexible brace and spacing rules aligned with modern C# practices
- **Documentation**: More relaxed documentation requirements for rapid development

#### Analysis Features
- **Code Style Enforcement**: Formatting and style rules are enforced at build time
- **Warning Level Enforcement**: All compiler warnings are treated as build errors
- **Latest Analysis Level**: Uses the most recent analyzer rule sets
- **Custom Ruleset**: `jetbrains-aligned.ruleset` provides JetBrains-compatible rule configuration

To check for static analysis issues locally:
```bash
# Build will fail if any warnings/analysis issues are found
dotnet build

# Or use the CI script which includes static analysis
script/cibuild
```

Common static analysis violations to avoid:
- Unused variables, fields, or methods
- Missing null checks where required
- Unreachable code
- Code style violations
- Potential security issues flagged by analyzers

#### IDE Integration
For the best development experience with JetBrains-style analysis:
- **JetBrains Rider**: Full integration with enhanced inspection and refactoring
- **ReSharper (Visual Studio)**: Complete analysis with real-time feedback
- **Visual Studio**: Basic integration via configured analyzers
- **VS Code**: Analyzer support through C# extension

#### Configuration Files
- `Directory.Build.props`: Project-wide analyzer configuration
- `jetbrains-aligned.ruleset`: JetBrains-compatible rule definitions
- `stylecop.json`: StyleCop analyzer settings
- `.editorconfig`: Code formatting standards

### Project Conventions
- Namespaces should follow the folder structure
- Use dependency injection for services
- Implement proper disposal patterns where needed
- Follow the established separation between presentation and business logic

#### Coordinate System and Positioning
For any code involving graphical positioning, layout, or coordinate calculations:
- **Use CoordinateHelper utilities** instead of direct coordinate math to avoid left/right confusion
- **Reference COORDINATE_SYSTEM.md** for understanding the coordinate system rules
- **Remember**: LEFT = LOWER X coordinates, RIGHT = HIGHER X coordinates
- **Use explicit methods** like `CoordinateHelper.MoveToTheRight()` instead of ambiguous arithmetic
- **Test positioning logic** with the coordinate system test patterns for validation

### Testing Best Practices

#### Unit Testing Guidelines
- **One Test, One Assert**: Each test method should focus on a single assertion to improve test clarity and make failure diagnosis easier
- **Use Parametric Tests**: Leverage xUnit's `[Theory]` and `[InlineData]` attributes to test multiple scenarios without duplicating test logic
- **Descriptive Test Names**: Write test names that clearly indicate what is being tested and the expected outcome
- **Arrange-Act-Assert Pattern**: Structure tests with clear sections for setup, execution, and verification

##### Examples

**Single Assert per Test:**
```csharp
[Fact]
public void SelectGate_ShouldSetSelectedGate()
{
    // Arrange
    var viewModel = CreateTestViewModel();
    
    // Act
    viewModel.SelectGate("OR");
    
    // Assert
    Assert.Equal("OR", viewModel.SelectedGate);
}

[Fact]
public void SelectGate_ShouldSetDraggingState()
{
    // Arrange
    var viewModel = CreateTestViewModel();
    
    // Act
    viewModel.SelectGate("OR");
    
    // Assert
    Assert.True(viewModel.IsDragging);
}
```

**Parametric Tests for Multiple Scenarios:**
```csharp
[Theory]
[InlineData("OR")]
[InlineData("AND")]
[InlineData("NOT")]
public void SelectGate_ShouldSetSelectedGateForDifferentTypes(string gateType)
{
    // Arrange
    var viewModel = CreateTestViewModel();
    
    // Act
    viewModel.SelectGate(gateType);
    
    // Assert
    Assert.Equal(gateType, viewModel.SelectedGate);
}
```

#### UI and Integration Testing
- Use specific element selectors in tests rather than generic ones to avoid ambiguity as the application grows
- Add data attributes (e.g., `data-testid`, `data-gate-type`) to elements that need to be tested for reliable selection
- Extract repeated inline styles to CSS classes for better maintainability
- Ensure tests target the specific functionality being tested rather than relying on implementation details

## Building and Testing

### Building the Solution
```bash
# Using standard scripts (recommended)
script/update  # Install dependencies and build

# Or manually using dotnet
dotnet build

# Build specific project
dotnet build AiLogica.Core

# Build for release (includes static analysis)
dotnet build -c Release
```

**Note**: All builds automatically include static analysis checks. Builds will fail if any warnings or code quality issues are detected.

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

# Run tests excluding End-to-End tests (faster for development)
dotnet test --filter "Category!=EndToEnd"

# Run only End-to-End tests
dotnet test --filter "Category=EndToEnd"
```

### End-to-End Testing

The project includes comprehensive End-to-End tests using Playwright for browser automation. These tests validate complete user workflows and ensure the application works correctly from the user's perspective.

```bash
# Set up and run E2E tests (automated script)
./script/run-e2e-tests.sh

# Manual E2E test setup
dotnet build --configuration Release
pwsh ./AiLogica.Tests/bin/Release/net8.0/playwright.ps1 install firefox
dotnet test --filter "Category=EndToEnd" --configuration Release
```

For detailed information about End-to-End testing, see [E2E_TESTING_GUIDE.md](E2E_TESTING_GUIDE.md).

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

## Requirements Traceability

### Overview

This project maintains comprehensive requirements traceability to support robust development methodology. All functional requirements defined in `REQUIREMENTS.md` are tracked through implementation and testing in `TRACEABILITY_MATRIX.md`.

### Developer Responsibilities

**Every developer must maintain requirements traceability by:**

1. **Updating REQUIREMENTS.md** when implementing new functionality that requires new requirements
2. **Updating TRACEABILITY_MATRIX.md** whenever:
   - New requirements are added
   - New functionality is implemented  
   - Existing functionality is modified
   - Code files are refactored or moved
   - Test coverage changes

3. **Adding traceability comments** to source code where they provide value without cluttering the codebase

### Traceability Comment Guidelines

**DO add traceability comments when:**
- Implementing a core functional requirement (FR-x)
- The code represents a significant requirement implementation
- It helps future developers understand requirement coverage
- The comment is brief and adds value

**DON'T add traceability comments when:**
- It would clutter code with obvious information
- The requirement mapping is already clear from context
- It would interfere with code readability

### Example Traceability Comments

```csharp
// FR-2.4: Gate placement by clicking on canvas
public void PlaceGate(double x, double y)

// FR-3.8: Generates orthogonal wire segments
private List<WireSegment> GenerateWireSegments()
```

```html
@* FR-2.1: Gate palette highlighting on selection *@
<div class="gate-item @(ViewModel.SelectedGate == "OR" ? "selected" : "")">
```

### Validation

Before submitting any PR that adds or modifies functionality:

1. **Review TRACEABILITY_MATRIX.md** to ensure it reflects your changes
2. **Run the complete test suite** to verify requirement coverage
3. **Update documentation** if new requirements were added

The traceability matrix serves as a living document that supports:
- Development planning and prioritization
- Quality assurance and testing
- Requirement coverage analysis
- Technical debt identification

## Next Steps for Development

Based on the requirements in REQUIREMENTS.md, the next major development tasks include:

1. **Gate Models**: Implement basic logic gate representations
2. **Canvas Component**: Create interactive drawing area for gate placement
3. **Simulation Engine**: Build logic evaluation and state management
4. **UI Components**: Develop Gate Palette and property panels
5. **Hierarchical Design**: Implement component abstraction and navigation

