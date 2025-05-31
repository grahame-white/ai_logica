using AiLogica.Core.ViewModels;

namespace AiLogica.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private string _welcomeMessage = "Logic Gate Design Canvas";
    private string? _selectedGate;
    private bool _isDragging;
    private double _mouseX;
    private double _mouseY;
    private List<PlacedGate> _placedGates = new();

    public string WelcomeMessage
    {
        get => _welcomeMessage;
        set => SetProperty(ref _welcomeMessage, value);
    }

    public string? SelectedGate
    {
        get => _selectedGate;
        set => SetProperty(ref _selectedGate, value);
    }

    public bool IsDragging
    {
        get => _isDragging;
        set => SetProperty(ref _isDragging, value);
    }

    public double MouseX
    {
        get => _mouseX;
        set => SetProperty(ref _mouseX, value);
    }

    public double MouseY
    {
        get => _mouseY;
        set => SetProperty(ref _mouseY, value);
    }

    public List<PlacedGate> PlacedGates
    {
        get => _placedGates;
        set => SetProperty(ref _placedGates, value);
    }

    public void SelectGate(string gateType)
    {
        SelectedGate = gateType;
        IsDragging = true;
        // Reset mouse position to avoid showing gate at stale coordinates
        MouseX = 0;
        MouseY = 0;
    }

    public void UpdateMousePosition(double x, double y)
    {
        MouseX = x;
        MouseY = y;
    }

    public void PlaceGate(double x, double y)
    {
        if (SelectedGate != null)
        {
            // Apply the same offset used during dragging to center the gate on cursor
            PlacedGates.Add(new PlacedGate
            {
                Type = SelectedGate,
                X = x - 30, // Same offset as dragging-gate positioning
                Y = y - 15, // Same offset as dragging-gate positioning
                Id = Guid.NewGuid()
            });
            OnPropertyChanged(nameof(PlacedGates));

            // Intentionally keep gate selected and dragging state active to allow 
            // users to place multiple gates of the same type without re-selecting.
            // This implements the requirement for continuous gate placement workflow.
            // To reset selection, users can click elsewhere or select a different gate.
        }
    }

    public void CancelDrag()
    {
        SelectedGate = null;
        IsDragging = false;
    }
}
