# AI Logica - Developer Guide

## Welcome to AI Logica Development

This guide provides comprehensive information for both human and AI developers contributing to the AI Logica project. The project is designed with AI-assisted development in mind, following clear patterns and conventions.

## Table of Contents
1. [Getting Started](#getting-started)
2. [Development Environment](#development-environment)
3. [Project Structure](#project-structure)
4. [Coding Standards](#coding-standards)
5. [AI Development Guidelines](#ai-development-guidelines)
6. [Testing Guidelines](#testing-guidelines)
7. [Contribution Workflow](#contribution-workflow)
8. [Common Development Tasks](#common-development-tasks)

## Getting Started

### Prerequisites
- .NET 8 SDK or later
- Visual Studio 2022, VS Code, or any text editor
- Git for version control
- Modern web browser for testing

### Quick Start
```bash
# Clone the repository
git clone https://github.com/grahame-white/ai_logica.git
cd ai_logica

# Build the solution
dotnet build

# Run tests
dotnet test

# Start the application
cd AiLogica
dotnet run
```

The application will be available at `https://localhost:5001` (or the port shown in console output).

## Development Environment

### Recommended IDE Setup
- **Visual Studio 2022**: Full IDE with excellent Blazor support
- **VS Code**: Lightweight with C# extension
- **JetBrains Rider**: Alternative full-featured IDE

### Useful Extensions
- C# for Visual Studio Code
- Blazor syntax highlighting
- GitLens for Git integration
- Live Share for collaborative development

### Browser Developer Tools
- Essential for debugging Blazor Server applications
- Use Network tab to monitor SignalR connections
- Console tab for JavaScript errors and logs

## Project Structure

### Solution Organization
```
AiLogica.sln
├── AiLogica/                 # Main web application
│   ├── Components/
│   │   ├── Layout/          # Layout components
│   │   ├── Pages/           # Page components
│   │   └── Shared/          # Shared components
│   ├── ViewModels/          # Presentation layer view models
│   ├── Services/            # Application services
│   ├── Models/              # Data transfer objects
│   └── wwwroot/             # Static assets
├── AiLogica.Core/           # Core business logic
│   ├── Models/              # Domain models
│   ├── Services/            # Business services
│   ├── Interfaces/          # Service contracts
│   └── ViewModels/          # Base view models
└── AiLogica.Tests/          # Test projects
    ├── Unit/                # Unit tests
    ├── Integration/         # Integration tests
    └── UI/                  # UI tests
```

### Key Conventions
- **Namespaces**: Follow folder structure
- **Components**: PascalCase, descriptive names
- **Services**: Interface + Implementation pattern
- **ViewModels**: Inherit from `ViewModelBase`

## Coding Standards

### C# Conventions
```csharp
// ✅ Good: Clear interface definition
public interface IGateService
{
    Task<Gate> CreateGateAsync(GateType type, Point position);
    Task DeleteGateAsync(string gateId);
}

// ✅ Good: Proper async/await usage
public async Task<Circuit> LoadCircuitAsync(string filename)
{
    var json = await File.ReadAllTextAsync(filename);
    return JsonSerializer.Deserialize<Circuit>(json);
}

// ✅ Good: Null checking and validation
public void SetGatePosition(Gate gate, Point position)
{
    ArgumentNullException.ThrowIfNull(gate);
    
    if (position.X < 0 || position.Y < 0)
        throw new ArgumentException("Position must be non-negative");
    
    gate.Position = position;
    OnGatePositionChanged(gate);
}
```

### Blazor Component Guidelines
```razor
@* ✅ Good: Clear component structure *@
@page "/circuit/{CircuitId}"
@using AiLogica.ViewModels
@inject ICircuitService CircuitService

<div class="circuit-designer">
    <GatePalette OnGateSelected="HandleGateSelected" />
    <DesignCanvas Circuit="@Circuit" OnGateDropped="HandleGateDropped" />
    <PropertiesPanel SelectedGate="@SelectedGate" />
</div>

@code {
    [Parameter] public string CircuitId { get; set; } = string.Empty;
    
    private Circuit? Circuit { get; set; }
    private Gate? SelectedGate { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        Circuit = await CircuitService.GetCircuitAsync(CircuitId);
    }
}
```

### CSS Conventions
```css
/* ✅ Good: BEM-style naming */
.gate-palette {
    background-color: var(--panel-background);
    border-right: 1px solid var(--border-color);
}

.gate-palette__category {
    margin-bottom: 1rem;
}

.gate-palette__item {
    padding: 0.5rem;
    cursor: pointer;
    transition: background-color 0.2s ease;
}

.gate-palette__item--selected {
    background-color: var(--selection-color);
}
```

## AI Development Guidelines

### AI-Friendly Code Patterns

#### Clear Intent Through Naming
```csharp
// ✅ AI-Friendly: Intent is clear from name
public class LogicGateEvaluator
{
    public bool EvaluateAndGate(bool input1, bool input2) => input1 && input2;
    public bool EvaluateOrGate(bool input1, bool input2) => input1 || input2;
    public bool EvaluateNotGate(bool input) => !input;
}

// ❌ AI-Unfriendly: Unclear purpose
public class Processor
{
    public bool Process(bool a, bool b) => a && b;
}
```

#### Documentation for AI Context
```csharp
/// <summary>
/// Represents a logic gate in the circuit designer.
/// This is the base class for all gate types (AND, OR, NOT, etc.).
/// Each gate has inputs, outputs, and evaluation logic.
/// </summary>
/// <remarks>
/// Gates are the fundamental building blocks of digital circuits.
/// They process boolean inputs to produce boolean outputs according to
/// their specific logic function.
/// </remarks>
public abstract class Gate
{
    /// <summary>
    /// Evaluates the gate's logic function based on current input values.
    /// This method should be called whenever input values change.
    /// </summary>
    /// <returns>True if any output values changed, false otherwise</returns>
    public abstract bool Evaluate();
}
```

#### Predictable Error Handling
```csharp
// ✅ Consistent error handling pattern
public class CircuitValidator
{
    public ValidationResult ValidateCircuit(Circuit circuit)
    {
        var result = new ValidationResult();
        
        if (circuit == null)
        {
            result.AddError("Circuit cannot be null");
            return result;
        }
        
        ValidateGates(circuit.Gates, result);
        ValidateConnections(circuit.Connections, result);
        
        return result;
    }
    
    private void ValidateGates(IEnumerable<Gate> gates, ValidationResult result)
    {
        foreach (var gate in gates)
        {
            if (string.IsNullOrEmpty(gate.Id))
                result.AddError($"Gate {gate.Type} is missing an ID");
        }
    }
}
```

### AI Assistance Integration Points

#### Code Generation Targets
```csharp
// Areas suitable for AI code generation:

// 1. Gate implementations (follow pattern)
public class XorGate : BasicGate
{
    protected override bool EvaluateLogic(bool[] inputs)
    {
        return inputs[0] ^ inputs[1];
    }
}

// 2. Test cases (follow template)
[Fact]
public void XorGate_ShouldReturnTrue_WhenInputsDiffer()
{
    // Arrange
    var gate = new XorGate();
    gate.SetInput(0, true);
    gate.SetInput(1, false);
    
    // Act
    var result = gate.Evaluate();
    
    // Assert
    Assert.True(gate.GetOutput(0));
}

// 3. Serialization/Deserialization
public class CircuitSerializer
{
    public string SerializeToJson(Circuit circuit) { /* Pattern implementation */ }
    public Circuit DeserializeFromJson(string json) { /* Pattern implementation */ }
}
```

### AI Development Best Practices

1. **Follow Established Patterns**: Look for existing implementations to understand patterns
2. **Maintain Consistency**: Use the same naming conventions and structure
3. **Add Comprehensive Tests**: Every new feature should have corresponding tests
4. **Document Intent**: Explain the "why" not just the "what"
5. **Validate Assumptions**: Check your understanding against existing code

## Testing Guidelines

### Test Structure
```csharp
// ✅ Good test structure
public class GateServiceTests
{
    private readonly IGateService _gateService;
    private readonly Mock<ICircuitRepository> _mockRepository;
    
    public GateServiceTests()
    {
        _mockRepository = new Mock<ICircuitRepository>();
        _gateService = new GateService(_mockRepository.Object);
    }
    
    [Fact]
    public async Task CreateGateAsync_ShouldReturnGate_WhenValidInput()
    {
        // Arrange
        var gateType = GateType.And;
        var position = new Point(10, 20);
        
        // Act
        var gate = await _gateService.CreateGateAsync(gateType, position);
        
        // Assert
        Assert.NotNull(gate);
        Assert.Equal(gateType, gate.Type);
        Assert.Equal(position, gate.Position);
    }
}
```

### Test Categories
- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions
- **UI Tests**: Test Blazor component behavior
- **End-to-End Tests**: Test complete user scenarios

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test AiLogica.Tests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run tests matching pattern
dotnet test --filter "Name~Gate"
```

## Contribution Workflow

### Branch Strategy
- `main`: Stable release branch
- `develop`: Integration branch for features
- `feature/*`: Individual feature branches
- `bugfix/*`: Bug fix branches
- `hotfix/*`: Critical production fixes

### Pull Request Process
1. **Create Feature Branch**: `git checkout -b feature/new-gate-type`
2. **Implement Changes**: Follow coding standards and add tests
3. **Run Tests**: Ensure all tests pass
4. **Create Pull Request**: Clear description of changes
5. **Code Review**: Address feedback from reviewers
6. **Merge**: Squash commits for clean history

### Commit Message Format
```
type(scope): short description

Longer description if needed.

- Bullet points for multiple changes
- Reference issues: Fixes #123
```

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

## Common Development Tasks

### Adding a New Gate Type
1. **Create Gate Class**: Inherit from appropriate base class
2. **Implement Logic**: Override evaluation methods
3. **Add to Factory**: Register in gate factory
4. **Create Tests**: Unit tests for logic
5. **Update UI**: Add to gate palette
6. **Document**: Update user documentation

### Adding New UI Components
1. **Create Component**: Follow naming conventions
2. **Define Parameters**: Use `[Parameter]` attributes
3. **Implement Logic**: Use code-behind or view models
4. **Add Styling**: Create component-specific CSS
5. **Create Tests**: Blazor component tests
6. **Integration**: Wire into parent components

### Debugging Tips

#### Blazor Server Debugging
```csharp
// Add breakpoints in C# code
public void OnGateSelected(GateType gateType)
{
    System.Diagnostics.Debugger.Break(); // Temporary breakpoint
    SelectedGateType = gateType;
}

// Use logging for runtime debugging
_logger.LogInformation("Gate {GateType} selected at position {Position}", 
                       gateType, position);
```

#### JavaScript Interop Debugging
```javascript
// In browser console
window.blazorCulture = {
    get: () => 'en-US',
    set: (value) => console.log('Culture set to:', value)
};
```

### Performance Optimization

#### Blazor Component Optimization
```csharp
// Use ShouldRender to control re-rendering
protected override bool ShouldRender()
{
    return _needsRerender;
}

// Implement IDisposable for cleanup
public void Dispose()
{
    _timer?.Dispose();
    _eventSubscription?.Dispose();
}
```

#### Memory Management
```csharp
// Use object pooling for frequently created objects
private readonly ObjectPool<Gate> _gatePool;

public Gate CreateGate(GateType type)
{
    var gate = _gatePool.Get();
    gate.Initialize(type);
    return gate;
}

public void ReturnGate(Gate gate)
{
    gate.Reset();
    _gatePool.Return(gate);
}
```

## Getting Help

### Resources
- **Documentation**: Check the `/docs` folder for detailed specifications
- **Issues**: Report bugs and request features on GitHub
- **Discussions**: Use GitHub Discussions for questions
- **Code Reviews**: Learn from existing pull requests

### Contact Information
- **Project Maintainer**: [GitHub Profile]
- **Issue Tracker**: [GitHub Issues]
- **Community**: [GitHub Discussions]

### AI Development Support
When working as an AI developer:
1. **Examine Existing Code**: Understand patterns before implementing
2. **Follow Test Patterns**: Look at existing tests for examples
3. **Ask for Clarification**: Use GitHub issues for architectural questions
4. **Validate Understanding**: Test your assumptions with sample implementations