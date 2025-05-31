# End-to-End Test Configuration

## Configuration Options

The E2E tests can be configured through environment variables and settings:

### Browser Configuration
- `E2E_HEADLESS`: Set to "false" to run tests with visible browser (default: true)
- `E2E_BROWSER`: Browser to use - "chromium", "firefox", or "webkit" (default: chromium)
- `E2E_VIEWPORT_WIDTH`: Browser viewport width (default: 1280)
- `E2E_VIEWPORT_HEIGHT`: Browser viewport height (default: 720)

### Test Configuration
- `E2E_TIMEOUT`: Global timeout for test operations in milliseconds (default: 5000)
- `E2E_SCREENSHOT_PATH`: Directory for test screenshots (default: /tmp)

### Environment Examples

#### Local Development with Visible Browser
```bash
export E2E_HEADLESS=false
dotnet test --filter "Category=EndToEnd"
```

#### CI Environment (Default)
```bash
# Uses headless browser with CI-friendly options
dotnet test --filter "Category=EndToEnd"
```

#### Custom Viewport for Different Screen Sizes
```bash
export E2E_VIEWPORT_WIDTH=1920
export E2E_VIEWPORT_HEIGHT=1080
dotnet test --filter "Category=EndToEnd"
```

## BDD Test Scenarios

The E2E tests are organized to support BDD-style scenarios. Each test method follows a clear pattern:

### Test Naming Convention
- `Feature_Action_ExpectedResult`: Clear description of what is being tested
- Example: `OrGateSelection_ShouldHighlightGateAndEnableDragging`

### Test Structure
1. **Arrange**: Set up the test environment and navigate to required page
2. **Act**: Perform user actions (click, hover, type, etc.)
3. **Assert**: Verify expected outcomes with clear error messages

### Common Scenarios Covered
- Application startup and component loading
- User interface interactions (clicking, hovering)
- Visual feedback and state changes
- Workflow completion and data persistence
- Error handling and edge cases

### Extending with Custom Scenarios
To add new BDD scenarios:
1. Create descriptive test method names
2. Use clear arrange-act-assert structure
3. Add comments explaining the user story
4. Include both positive and negative test cases
5. Ensure deterministic and isolated execution