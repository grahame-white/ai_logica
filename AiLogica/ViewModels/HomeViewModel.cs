using AiLogica.Core.ViewModels;

namespace AiLogica.ViewModels
{
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
                PlacedGates.Add(new PlacedGate
                {
                    Type = SelectedGate,
                    X = x,
                    Y = y,
                    Id = Guid.NewGuid()
                });
                OnPropertyChanged(nameof(PlacedGates));
                // Keep gate selected and dragging for multiple placement
                // SelectedGate = null;
                // IsDragging = false;
            }
        }

        public void CancelDrag()
        {
            SelectedGate = null;
            IsDragging = false;
        }
    }

    public class PlacedGate
    {
        public string Type { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
        public Guid Id { get; set; }
    }
}