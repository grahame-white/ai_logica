# AI Logica - Requirements Specification

## Important Note for AI Developers
**This requirements documentation must be updated to include any new requirements as they are added to the application. When implementing new functionality, ensure that corresponding functional requirements are documented in this file to maintain traceability and completeness of the specification.**

## 1. Core Functional Requirements

Based on the application outline, AI Logica shall provide the following capabilities:

### 1.1 Graphical Gate Layout
- **FR-1**: The end user will be able to graphically layout logic gates on a drawing area
- **FR-2**: It will be easy for the end user to select which gates to place onto the drawing area
- **FR-2.1**: When the end user selects a gate from the palette, the gate in the palette shall be highlighted to indicate selection
- **FR-2.2**: When the end user selects any other gate from the palette after selecting a gate, the previously selected gate in the palette shall not be highlighted
- **FR-2.3**: The selected gate shall follow the mouse cursor over the canvas area to provide visual feedback during placement
- **FR-2.4**: The end user shall be able to place the selected gate by clicking on the canvas at the desired location
- **FR-2.5**: The end user shall be able to place multiple gates of the same type without having to reselect the gate from the palette each time

#### 1.1.1 Gate Visual Design Standards
- **FR-2.6**: All logic gates shall be displayed using symbolic SVG representations following IEEE standard conventions
- **FR-2.7**: Gate symbols shall use vector graphics (SVG) for scalable, crisp rendering at any size
- **FR-2.8**: Gate symbols shall use `currentColor` for stroke color to inherit proper text color from CSS themes
- **FR-2.9**: Gates shall have consistent sizing with two size variants:
  - Palette gates: 32x24 pixels for efficient gate selection
  - Placed gates: 48x36 pixels for easier wire connection operations
- **FR-2.10**: Gate symbols shall maintain IEEE standard proportions (4:3 width-to-height ratio)
- **FR-2.11**: Gate symbols shall include proper input and output connection lines positioned for straight-line wire connections
- **FR-2.12**: Gate visual design shall use minimal internal margins (0.125rem 0.25rem for placed gates, 0.25rem for palette gates) to maximize symbol visibility
- **FR-2.13**: Line width for gate symbols shall be 1 pixel and shall not scale with canvas zoom (future feature consideration)
- **FR-2.14**: Gates shall display without background shading or border outlines to maintain clean appearance

### 1.2 Gate Wiring
- **FR-3**: The end user will be able to wire the inputs and outputs of the gates together
- **FR-3.1**: Placed gates shall be 100% larger than palette gates (96x72 pixels instead of 48x36 pixels) to make wire connections less fiddly
- **FR-3.2**: Each gate shall have visible connection points for inputs and outputs
- **FR-3.3**: Connection points shall be visually distinct with different colors for inputs (red) and outputs (green)
- **FR-3.4**: The end user shall be able to drag a new wire from one connection point to another
- **FR-3.5**: It shall be possible to wire an output connection to an input connection
- **FR-3.6**: It shall be possible to wire an input connection to an input connection (fan-out capability)
- **FR-3.7**: It shall be possible to wire connections within the same gate to enable feedback loops
- **FR-3.8**: Wires shall be drawn using as few orthogonal straight line segments as possible
- **FR-3.9**: Wire routing shall avoid passing through gates when possible
- **FR-3.10**: Connected wires shall be visually distinct from disconnected or incomplete wire connections
- **FR-3.11**: Connected wires shall be displayed in blue (#5eb3f5) color
- **FR-3.12**: Disconnected or incomplete wires shall be displayed in red (#f44336) color with dashed lines
- **FR-3.13**: Active wiring operations (wire being dragged) shall show a preview line from the source connection to the mouse cursor
- **FR-3.14**: The end user shall be able to cancel an active wiring operation by clicking on empty canvas space or pressing Escape
- **FR-3.15**: Connection points shall provide hover feedback to indicate they are interactive
- **FR-3.16**: Wire segments shall only be created when there is meaningful distance between connection points

### 1.3 Simulation Capabilities
- **FR-4**: The application will be able to simulate the behavior of the connected logic gates
- **FR-5**: The end user will be able to step through updates incrementally

### 1.4 Hierarchical Design
- **FR-6**: The end user will be able to save their layouts so that they can be used as a black box component in another layout
- **FR-7**: The end user will be able to drill down into black box component so that they can see inside it
- **FR-8**: The end user will be able to drill up from a black box component so that they can see how it is being used

### 1.5 Design Overview
- **FR-9**: The end user should be able to see an overview of their design from the most abstract level down to the most primitive logic gates

## 2. Technical Requirements

### 2.1 Platform
- Web-based application using Blazor Server and .NET 8
- Browser compatibility for modern desktop browsers
- Responsive design for various screen sizes

### 2.2 Performance
- Real-time simulation updates
- Smooth user interactions for gate placement and wiring
- Efficient rendering of complex circuit layouts

### 2.3 Usability
- Intuitive drag-and-drop interface
- Clear visual feedback for all operations
- Minimal learning curve for new users

### 2.4 Project Infrastructure
- **FR-10.1**: The project infrastructure shall make it easy for users to contribute across different capacities (feature requests, development, bug reporting, etc.)
- **FR-10.2**: The project infrastructure shall ensure that developers, Human and AI, are provided with the information they need to implement issues they are assigned
- **FR-10.3**: The project infrastructure shall help requesters express the problem that they are trying to solve through appropriate issue templates
- **FR-10.4**: The project infrastructure shall ensure that developers have the freedom to explore innovative solutions that solve user issues
- **FR-10.5**: Issue templates shall exist for each top-level ticket type (bug reports, feature requests, documentation issues, questions/support)
- **FR-10.6**: Each issue template shall prompt the requester to provide the details most likely to allow the desired outcome
- **FR-10.7**: Required fields in issue templates shall be carefully considered so that only information that a requester is reasonably likely to have is asked for

## 3. Conflicting Requirements and Trade-offs

As requested in the documentation requirements, the following conflicting requirements must be highlighted for appropriate direction:

### 3.1 Simplicity vs. Power
**Conflict**: Easy gate selection and placement vs. comprehensive gate library
- **Trade-off**: Start with basic gates (AND, OR, NOT) and expand based on user needs
- **Decision**: Focus on the most primitive gates. When more complex gates are wanted they will be explicitly requested

### 3.2 Visual Feedback vs. Performance
**Conflict**: Rich visual simulation feedback vs. real-time performance
- **Trade-off**: Optimize rendering for small circuits, provide performance warnings for large ones
- **Decision**: Focus on visual simulation feedback, keep on top of developing performance issues as additional features are added

### 3.3 Educational Focus vs. Professional Use
**Conflict**: Simple learning interface vs. advanced design capabilities
- **Trade-off**: Progressive interface complexity based on user experience level
- **Decision**: Prefer a simple interface, if more advanced use is required it will be asked for explicitly

### 3.4 Cross-Platform vs. Feature Richness
**Conflict**: Browser compatibility vs. advanced interactive features
- **Trade-off**: Use standard web technologies with graceful feature degradation
- **Decision**: Prefer browser compatibility, however if there is a significant opportunity to gain performance then request a decision