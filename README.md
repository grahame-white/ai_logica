# AI Logica

An exploration of using an AI agent to develop a logic gate simulator.

## Project Overview

AI Logica is a web-based application for designing and simulating digital logic circuits. The project serves as both an educational tool for digital logic concepts and an exploration of AI-assisted software development.

## Vision

The application aims to provide users with the ability to:
- Graphically layout logic gates on a drawing area
- Easily select and place gates from a palette
- Wire gate inputs and outputs together  
- Simulate the behavior of connected logic gates
- Step through simulation updates incrementally
- Save layouts as reusable black box components
- Drill down into components to see internal structure
- Drill up to see component usage context
- View design hierarchy from abstract to primitive levels

## Current Status

This project is in early development. The current implementation includes:
- ✅ Basic Blazor Server application structure (.NET 8)
- ✅ Multi-project solution organization
- ✅ MVVM foundation with ViewModelBase implementation
- ✅ Comprehensive documentation covering vision, requirements, and architecture
- ✅ Test project setup

## Getting Started

### Prerequisites
- .NET 8 SDK
- Modern web browser

### Running the Application
```bash
git clone https://github.com/grahame-white/ai_logica.git
cd ai_logica
dotnet build
cd AiLogica
dotnet run
```

Open your browser to `https://localhost:5001` to access the application.

## Documentation

- [**VISION.md**](VISION.md) - Application vision, mission, and target audiences
- [**REQUIREMENTS.md**](REQUIREMENTS.md) - Functional requirements based on application outline
- [**ARCHITECTURE.md**](ARCHITECTURE.md) - Current technical architecture and planned structure  
- [**DEVELOPER_GUIDE.md**](DEVELOPER_GUIDE.md) - Development setup and guidelines

## Technology Stack

- **Frontend**: Blazor Server Components, HTML5, CSS3
- **Backend**: .NET 8, ASP.NET Core  
- **Testing**: xUnit
- **Architecture**: MVVM pattern with clean separation of concerns

## Contributing

This project welcomes contributions from both human and AI developers. See [DEVELOPER_GUIDE.md](DEVELOPER_GUIDE.md) for development setup and guidelines.

## License

This project is licensed under the [MIT License](LICENSE).
