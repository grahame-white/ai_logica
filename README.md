# AI Logica - Logic Gate Simulator

AI Logica is an intuitive, web-based logic gate simulator designed for education, prototyping, and understanding digital logic circuits. Built with Blazor Server and .NET 8, it provides a rich interactive experience for designing and simulating digital logic circuits.

## 🎯 Vision

To democratize digital logic education and prototyping by providing a visual, interactive platform that makes digital circuit design accessible to students, educators, and professionals.

## ✨ Key Features

- **Visual Design**: Drag-and-drop interface for placing and connecting logic gates
- **Real-time Simulation**: Interactive simulation with step-by-step execution
- **Hierarchical Design**: Create reusable components and manage complex designs
- **Educational Focus**: Perfect for learning digital logic concepts
- **Modern Web Technology**: No installation required, runs in any modern browser

## 🚀 Quick Start

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

Open your browser to `https://localhost:5001` to start designing circuits!

## 🏗️ Current Status

This project is under active development. The foundation is in place with:
- ✅ Basic Blazor Server application structure
- ✅ MVVM architecture with clean separation of concerns
- ✅ Responsive UI layout with three-panel design
- ✅ Comprehensive test framework
- ✅ Complete documentation suite

### Planned Features
- Logic gate palette with drag-and-drop functionality
- Interactive canvas for circuit design
- Real-time simulation engine
- Component hierarchy and abstraction
- Save/load functionality
- Export capabilities

## 📚 Documentation

### For End Users
- [Application Vision](VISION.md) - Project goals and principles
- [Feature Requirements](REQUIREMENTS.md) - Detailed functional specifications

### For Developers
- [Technical Architecture](ARCHITECTURE.md) - System design and structure
- [Developer Guide](DEVELOPER_GUIDE.md) - Development setup and guidelines
- [API Documentation](docs/api/) - Code documentation (coming soon)

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 🤝 Contributing

We welcome contributions from both human and AI developers! This project is designed with AI-assisted development in mind.

### Getting Started
1. Read the [Developer Guide](DEVELOPER_GUIDE.md)
2. Check the [Requirements](REQUIREMENTS.md) for feature specifications
3. Review the [Architecture](ARCHITECTURE.md) to understand the system design
4. Browse existing issues for contribution opportunities

### Development Process
1. Fork the repository
2. Create a feature branch (`feature/your-feature-name`)
3. Follow the coding standards in the Developer Guide
4. Add tests for new functionality
5. Submit a pull request

## 📋 Project Structure

```
ai_logica/
├── AiLogica/              # Main Blazor Server application
├── AiLogica.Core/         # Core business logic and models
├── AiLogica.Tests/        # Test suite
├── docs/                  # Additional documentation
└── *.md                   # Project documentation
```

## 🛠️ Technology Stack

- **Frontend**: Blazor Server, HTML5 Canvas, CSS3
- **Backend**: .NET 8, ASP.NET Core
- **Testing**: xUnit, Blazor Testing Framework
- **Architecture**: MVVM, Component-based design

## 📖 Educational Goals

AI Logica is designed to support:
- **Digital Logic Courses**: Visual representation of logic concepts
- **Computer Architecture**: Understanding of digital system design
- **Self-Learning**: Interactive exploration of digital circuits
- **Prototyping**: Quick validation of logic circuit ideas

## 🌟 AI-Assisted Development

This project embraces AI-assisted development practices:
- Clear code patterns and conventions
- Comprehensive documentation for AI context
- Well-defined interfaces and abstractions
- Extensive test coverage for validation

## 📄 License

This project is licensed under the [MIT License](LICENSE).

## 🙏 Acknowledgments

This project explores the potential of AI-assisted software development while creating a valuable educational tool for digital logic learning.
