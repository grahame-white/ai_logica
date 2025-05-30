using AiLogica.Core.ViewModels;

namespace AiLogica.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private string _welcomeMessage = "Logic Gate Design Canvas";

        public string WelcomeMessage
        {
            get => _welcomeMessage;
            set => SetProperty(ref _welcomeMessage, value);
        }
    }
}