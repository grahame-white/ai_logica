# End-to-End Testing Guide

This document provides comprehensive guidance for end-to-end testing in the AI Logica project using Playwright.

## Overview

The end-to-end testing infrastructure validates complete user workflows by automating real browser interactions. This ensures that the application works correctly from the user's perspective and catches integration issues that unit tests might miss.

## Architecture

### Test Structure
- **EndToEndTestBase**: Base class providing common browser automation setup
- **OrGateWorkflowTests**: Core E2E tests for OR gate functionality
- **EndToEndInfrastructureTests**: Tests that verify the E2E testing setup

### Technology Stack
- **Playwright**: Browser automation framework
- **System Firefox**: Uses system-installed Firefox browser (with Playwright fallback)
- **xUnit**: Test framework with trait-based categorization
- **ASP.NET Core Testing**: Integration with WebApplicationFactory
- **Virtual Display (Xvfb)**: For headless testing in CI environments

## Running End-to-End Tests

### Local Development

#### Prerequisites
- .NET 8 SDK
- Firefox browser (installed automatically by setup script)
- Xvfb (for headless environments - installed automatically on Linux)

#### Quick Start
```bash
# Check E2E testing readiness (quick status check)
./script/check-e2e-status

# Run the automated E2E setup script
./script/run-e2e-tests.sh
```

#### Using the Main Test Script
```bash
# Run all tests except E2E (fast feedback - default)
./script/test

# Run only infrastructure tests (no browser required)
./script/test --infrastructure

# Run only E2E tests (requires browser setup)
./script/test --e2e

# Run complete test suite including E2E
./script/test --all
```

#### Manual Setup
```bash
# Build the solution
dotnet build --configuration Release

# Install Playwright browsers
pwsh ./AiLogica.Tests/bin/Release/net8.0/playwright.ps1 install firefox

# Run E2E tests
dotnet test --filter "Category=EndToEnd" --configuration Release
```

#### Running Specific Test Categories
```bash
# Fast feedback during development (excludes E2E)
./script/test

# Run only infrastructure tests (no browser required)
./script/test --infrastructure

# Run only E2E tests (requires browser setup)
./script/test --e2e

# Run complete test suite including E2E
./script/test --all

# Legacy dotnet commands (also work)
dotnet test --filter "EndToEndInfrastructureTests"
dotnet test --filter "Category=EndToEnd"
dotnet test --filter "Category!=EndToEnd"
```

### CI/CD Environment

The GitHub Actions workflow automatically:
1. Runs unit and integration tests first
2. Sets up Playwright browsers in a separate job
3. Runs E2E tests with proper environment configuration
4. Uploads screenshots on test failures for debugging

## Test Categories

### Infrastructure Tests
These tests validate the E2E testing setup without requiring browser installation:
- **EndToEndTestInfrastructure_ShouldBeConfiguredCorrectly**: Verifies web application factory setup
- **PlaywrightPackage_ShouldBeAvailable**: Confirms Playwright dependency is correctly installed
- **EndToEndTestBase_ShouldBeInstantiable**: Validates test base class structure

### Browser-Based E2E Tests
These tests require browser installation and validate complete user workflows:
- **ApplicationHomePage_ShouldLoadSuccessfully**: Verifies application startup and component rendering
- **OrGateSelection_ShouldHighlightGateAndEnableDragging**: Tests OR gate selection interaction
- **OrGateDragging_ShouldFollowMouseMovement**: Validates dragging functionality
- **OrGatePlacement_ShouldPlaceGateAndUpdateStatus**: Tests gate placement workflow
- **OrGateSelection_CancelOnMouseLeave_ShouldClearSelection**: Verifies cancellation behavior
- **MultipleOrGatePlacement_ShouldAllowPlacingMultipleGates**: Tests multiple gate placement
- **EndToEndWorkflow_CompleteOrGateSelection_ShouldProvideConsistentUserExperience**: Complete acceptance test

## Test Features

### BDD-Style Test Organization
Tests are organized with descriptive names that follow Given-When-Then patterns:
- **Given**: Test arrangement (e.g., "Navigate to home page")
- **When**: User action (e.g., "Click OR gate button")  
- **Then**: Expected outcome (e.g., "Gate should be highlighted")

### AI-Readable Feedback
Tests provide structured output that AI developers can easily interpret:
- Clear assertion messages with context
- Descriptive test names explaining the scenario
- Detailed error messages with troubleshooting guidance
- Screenshots captured on failures for visual debugging

### Deterministic Execution
Tests are designed for consistent, reliable execution:
- Proper test isolation with fresh browser contexts
- Explicit waits for elements and state changes
- Cleanup handled automatically via IAsyncLifetime
- Environment-specific configuration (Testing vs Development)

## Configuration

### Browser Configuration
```csharp
Browser = await Playwright.Firefox.LaunchAsync(new BrowserTypeLaunchOptions
{
    Headless = true, // Set to false for debugging
    Args = new[] { "--no-sandbox", "--disable-dev-shm-usage" } // CI-friendly options
});
```

### Viewport and Context
```csharp
BrowserContext = await Browser.NewContextAsync(new BrowserNewContextOptions
{
    ViewportSize = new ViewportSize { Width = 1280, Height = 720 }
});
```

### Test Environment
The test environment uses:
- Reduced logging (Warning level and above)
- Testing environment configuration
- Isolated test server instances
- Automatic cleanup and disposal

## Debugging

### Local Debugging
1. Set `Headless = false` in EndToEndTestBase to see browser interactions
2. Add breakpoints in test methods
3. Use `await TakeScreenshotAsync("debug")` to capture specific moments
4. Check `/tmp/` directory for screenshot files

### CI Debugging
1. Check test output in GitHub Actions logs
2. Download screenshot artifacts when tests fail
3. Review test execution timing and error messages
4. Verify browser installation completed successfully

### Common Issues
- **Browser not found**: Run playwright install command
- **Element not found**: Check CSS selectors against actual HTML
- **Timing issues**: Increase timeout values or add explicit waits
- **Network issues**: Verify application is accessible on test port

## Extending E2E Tests

### Adding New Test Scenarios
1. Create test methods in OrGateWorkflowTests or new test classes
2. Use descriptive names following the established pattern
3. Add `[Trait("Category", "EndToEnd")]` attribute
4. Follow the Arrange-Act-Assert pattern
5. Use helper methods from EndToEndTestBase

### Adding New Test Categories
1. Create new test class inheriting from EndToEndTestBase
2. Add appropriate trait attributes for categorization
3. Update CI workflow if new categories require different setup
4. Document new test categories in this guide

### Best Practices
- **Test Independence**: Each test should be able to run independently
- **Clear Assertions**: Use descriptive assertion messages
- **Minimal Test Data**: Use the smallest dataset that validates the scenario
- **Error Handling**: Provide clear error messages for AI interpretation
- **Performance**: Balance thorough testing with reasonable execution time

## Integration with Development Workflow

### Pre-commit Validation
Developers can run different test suites before committing:
```bash
# Quick status check (validates readiness without running tests)
./script/check-e2e-status

# Quick validation (excludes E2E - recommended for most commits)
./script/test

# Infrastructure validation (checks E2E setup without browser)
./script/test --infrastructure  

# Full validation with E2E tests (thorough but slower)
./script/test --all

# Legacy approach using the dedicated E2E script
./script/run-e2e-tests.sh
```

### AI Developer Usage
AI developers can use E2E tests to:
- Validate functionality changes before pushing
- Generate AI-readable feedback about application behavior
- Understand expected user workflows through test scenarios
- Debug issues using structured test output and screenshots

### Continuous Integration
The CI pipeline ensures:
- E2E tests only run after unit tests pass
- Browser setup is handled automatically
- Test artifacts are preserved for debugging
- Failures provide actionable feedback

## Maintenance

### Updating Playwright
1. Update Microsoft.Playwright package version
2. Run `pwsh playwright.ps1 install` to update browsers
3. Test with latest browser versions
4. Update CI workflow if needed

### Updating Test Selectors
When HTML structure changes:
1. Update CSS selectors in test files
2. Verify selectors work across different viewport sizes
3. Test with both headless and headed browser modes
4. Update test documentation if behavior changes

### Performance Monitoring
Monitor E2E test execution time and optimize as needed:
- Use parallel test execution where appropriate
- Minimize unnecessary waits and delays
- Cache browser installations in CI
- Profile test execution to identify bottlenecks