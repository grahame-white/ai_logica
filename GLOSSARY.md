# AI Logica - Glossary of Terms

## Purpose

This glossary defines terminology used throughout the AI Logica project to ensure consistent communication between developers (human and AI), maintainers, and contributors. All project participants should refer to this glossary when working on issues and extend it when new terms are introduced or misunderstandings arise.

## How to Use This Glossary

- **For Developers**: Reference this glossary when assigned to issues to ensure consistent terminology
- **For Documentation**: Use these term definitions when writing or updating documentation  
- **For Implementation**: Follow these definitions when naming variables, methods, classes, and components
- **For Issues**: Use these terms when reporting bugs or requesting features

## Updating This Glossary

When extending this glossary:
1. Add new terms in the appropriate section in alphabetical order
2. Ensure definitions are clear and unambiguous
3. Review related documentation and implementation for consistency
4. Update the traceability matrix if applicable

---

## Technical Architecture Terms

### Backend
The server-side components of AI Logica, implemented using .NET 8 and ASP.NET Core. Responsible for business logic, data processing, and serving the Blazor Server application.

### Blazor Server
The web framework used for AI Logica's frontend. A server-side rendering technology where UI interactions are handled on the server and updates are sent to the browser via SignalR.

### Component
In Blazor context: A reusable UI element that encapsulates HTML, CSS, and C# logic. In simulation context: A reusable logic circuit element that can be saved and reused (see [Logic Component](#logic-component)).

### Frontend
The client-side components of AI Logica, consisting of Blazor Server components, HTML5, and CSS3 rendered in the user's web browser.

### MVVM (Model-View-ViewModel)
The architectural pattern used in AI Logica where:
- **Model**: Business logic and data (Core library)
- **View**: UI components (Blazor components)
- **ViewModel**: Presentation logic that binds Views to Models

### ViewModelBase
The base class that implements INotifyPropertyChanged for MVVM pattern support, providing the foundation for data binding between Views and business logic.

### ViewModels
Classes that inherit from ViewModelBase and provide presentation logic, handling the interaction between Blazor components (Views) and business logic (Models).

---

## Logic Simulation Terms

### Canvas
The main drawing area where users place and arrange logic gates. Also called the [Drawing Area](#drawing-area).

### Circuit
A complete collection of interconnected logic gates that forms a functional digital logic design. Can range from simple gate combinations to complex hierarchical designs.

### Connection Point
Specific locations on gates where wires can be attached. There are two types:
- **Input Connection Point**: Where signals enter a gate (visually marked in red)
- **Output Connection Point**: Where signals exit a gate (visually marked in green)

### Drawing Area
See [Canvas](#canvas).

### Fan-out
The capability to connect one output to multiple inputs, allowing a signal to drive multiple gate inputs simultaneously.

### Feedback Loop
A circuit configuration where the output of a gate or circuit is connected back to its own input, either directly or through other gates.

### Gate
A fundamental digital logic element that performs a boolean operation on its inputs to produce an output. Examples: AND, OR, NOT, NAND, NOR, XOR.

### Gate Body
The main visual component of a logic gate symbol that represents its distinctive shape and function. For example, the shield-like shape of an OR gate, the curved back of a NAND gate, or the triangular shape of a NOT gate. The gate body contains the core logic function and is standardized according to IEEE conventions.

### Gate Connector Lines
The straight line segments that extend from the gate body to provide visual pathways for signal connections. These lines connect the gate body to the [Connection Points](#connection-point) where wires can be attached, clearly indicating input and output locations on the gate symbol.

### Gate Inversion Bubble
A small circular symbol that indicates logical negation or inversion. These bubbles appear on logic gates to show that the output is inverted (e.g., NAND, NOR gates) or that specific inputs are inverted. Commonly positioned at the output of NOT gates or between the [Gate Body](#gate-body) and [Gate Connector Lines](#gate-connector-lines) on inverted gates. Also known as a **Negation Circle** in some contexts.

### Gate Palette
The UI area containing available gate types that users can select and place onto the canvas. Gates in the palette are smaller (32x24 pixels) than placed gates.

### Gate Placement
The action of selecting a gate from the palette and positioning it on the canvas by clicking at the desired location.

### Hierarchical Design
The capability to create complex circuits by:
- Saving circuit layouts as reusable [Logic Components](#logic-component)
- **Drilling down** into components to view internal structure
- **Drilling up** to see how components are used in larger designs

### Input
A signal entry point on a logic gate. Gates can have multiple inputs depending on their type.

### Logic Component
A saved circuit layout that can be reused as a "black box" component in other designs. Also called **Black Box Component**.

### Output
A signal exit point on a logic gate. Most basic gates have a single output.

### Placed Gate
A gate that has been positioned on the canvas. Placed gates are larger (48x36 pixels) than palette gates to facilitate easier wire connections.

### Signal
The logical state (high/low, true/false, 1/0) flowing through wires and processed by gates.

### Signal Flow
The direction and path that signals take through a circuit, from inputs through gates to outputs.

### Simulation
The process of calculating and displaying the logical behavior of a circuit, showing how signals propagate through connected gates.

### Wire
A connection between gate connection points that carries signals. Wires are drawn using orthogonal line segments and are color-coded:
- **Blue (#5eb3f5)**: Connected wires
- **Red (#f44336)**: Disconnected or incomplete wires (shown with dashed lines)

### Wire Routing
The algorithm that determines the path a wire takes between two connection points, typically using the minimum number of orthogonal segments while avoiding gates when possible.

---

## Development Process Terms

### AI Developer
An artificial intelligence agent (like GitHub Copilot) that contributes to the project by implementing features, fixing bugs, or improving documentation.

### Contributor
Any individual (human or AI) who submits code, documentation, or other improvements to the project.

### Developer
A general term for anyone working on the project, including both human developers and AI developers.

### Functional Requirement (FR)
A specific capability or behavior that the system must provide, identified with an ID (e.g., FR-1, FR-2.1) and tracked in REQUIREMENTS.md.

### Human Developer
A human contributor who works on the project alongside AI developers.

### Issue Template
Structured forms in GitHub that guide users to provide necessary information when reporting bugs, requesting features, or asking questions.

### Maintainer
A project team member with administrative access who reviews contributions, makes architectural decisions, and maintains project standards.

### Requirements Traceability
The practice of tracking functional requirements from specification through implementation and testing, documented in TRACEABILITY_MATRIX.md.

### Scripts to Rule Them All
The standardized development workflow pattern used in AI Logica, providing consistent commands (`script/setup`, `script/test`, `script/server`, etc.) regardless of underlying technology.

### Traceability Matrix
A document (TRACEABILITY_MATRIX.md) that maps functional requirements to their implementation files and test coverage.

---

## UI/UX Terms

### Canvas Interaction
User interactions with the main drawing area, including Gate Placement, selection, and wire creation.

### Hover Feedback
Visual indication when the user's cursor is positioned over an interactive element, such as connection points changing appearance when ready for wire attachment.

### Palette Selection
The process of choosing a gate type from the Gate Palette, which highlights the selected gate and prepares it for placement.

### Visual Feedback
Any visual indication that helps users understand the current state of their actions, such as:
- Selected gates following the mouse cursor
- Wire preview during active wiring operations
- Connection point highlighting during hover

### Wire Preview
The temporary line shown from a connection point to the mouse cursor during active wiring operations, helping users visualize the wire before completing the connection.

---

## Testing Terms

### bunit
The Blazor component testing framework used for testing Blazor components in isolation.

### End-to-End Testing (E2E)
Testing that verifies complete user workflows from start to finish, implemented using Playwright.

### Playwright
The browser automation framework used for End-to-End testing of the web application.

### Unit Testing
Testing individual components or methods in isolation, implemented using xUnit.

### xUnit
The .NET testing framework used for Unit Testing in the AI Logica project.

---

## Project Infrastructure Terms

### CI/CD
Continuous Integration and Continuous Deployment - the automated processes that build, test, and deploy the application.

### Git Hooks
Automated scripts that run at specific Git events (pre-commit, pre-push) to enforce code formatting and quality standards.

### GitHub Templates
See [Issue Template](#issue-template).

### Project Structure
The organization of files and directories in the AI Logica solution:
- `AiLogica/`: Main Blazor Server application
- `AiLogica.Core/`: Core business logic library  
- `AiLogica.Tests/`: Test project
- `script/`: Development workflow scripts

### Script Directory
The `script/` folder containing standardized development commands following the "Scripts to Rule Them All" pattern.

---

## Standards and Conventions

### IEEE Standard
The Institute of Electrical and Electronics Engineers standards for logic gate symbols, which AI Logica follows for gate visual representation.

### Naming Conventions
Established patterns for naming files, classes, methods, and other project elements to maintain consistency across the codebase.

### Pixel Sizing
Specific size standards for UI elements:
- Palette gates: 32x24 pixels
- Placed gates: 48x36 pixels (100% larger for easier interaction)

### SVG (Scalable Vector Graphics)
The vector graphics format used for gate symbols, ensuring crisp rendering at any size and proper scaling.

---

## Documentation Standards

All project documentation follows these principles:
- **Clear Language**: Use simple, precise terminology defined in this glossary
- **Consistency**: Apply terms consistently across all documentation
- **Cross-References**: Link related documents and concepts
- **Living Documents**: Keep documentation updated with implementation changes

When in doubt about terminology, refer to this glossary first, then propose updates if clarification is needed.