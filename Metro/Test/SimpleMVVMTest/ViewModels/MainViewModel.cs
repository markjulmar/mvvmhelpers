using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;
using Windows.UI.Popups;
using UICommand = JulMar.Windows.UI.UICommand;

namespace SimpleMvvmTest.ViewModels
{
    /// <summary>
    /// Simple ViewModel to do some testing.
    /// </summary>
    public class MainViewModel : ViewModel
    {
        private static readonly string[] Colors = 
        {
            "White", "Red", "Violet", "Blue", "White",
        };

        private string _color;
        private string _text;
        private bool _isChanging;
        private bool _showAdvanced;

        public string Text
        {
            get { return _text; }
            set { SetPropertyValue(ref _text, value); }
        }

        public string Color
        {
            get { return _color; }
            private set { SetPropertyValue(ref _color, value); }
        }

        public bool ShowAdvanced
        {
            get { return _showAdvanced; }
            set { SetPropertyValue(ref _showAdvanced, value); }
        }

        public ICommand ShowText { get; private set; }
        public ICommand MouseEnter { get; private set; }
        public ICommand MouseLeave { get; private set; }

        public MainViewModel()
        {
            Color = "White";

            ShowText = new DelegateCommand(OnClickButton);
            MouseEnter = new AsyncDelegateCommand(OnGoForward, () => !_isChanging, () => _isChanging = false);
            MouseLeave = new AsyncDelegateCommand(OnGoBackward, () => !_isChanging, () => _isChanging = false);
        }

        public void JustShowText()
        {
            Text = "Hello from JustShowText";
        }

        // CallMethodAction prefers this form
        //public void JustShowText(object sender, object e)
        //{
        //    Text = sender.GetType() + " : " + e.GetType();
        //}

        private async void OnClickButton()
        {
            Text = "Hello from the Button.";

            if (ShowAdvanced)
            {
                IUICommand result = await ServiceProvider.Resolve<IMessageVisualizer>()
                    .ShowAsync("Advanced Hello Metro", "Select one of the buttons below.", new MessageVisualizerOptions(UICommand.YesNoCancel));
                Text += " You picked: " + result.Label;
            }
            else
            {
                await ServiceProvider.Resolve<IMessageVisualizer>()
                    .ShowAsync("Hello Metro", "A Message Prompt with OK");
            }
        }

        private async void OnGoForward()
        {
            _isChanging = true;
            foreach (var color in Colors)
            {
                Color = color;
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }
        }

        private async void OnGoBackward()
        {
            _isChanging = true;
            foreach (var color in Colors.Reverse())
            {
                Color = color;
                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }
        }
    }
}
