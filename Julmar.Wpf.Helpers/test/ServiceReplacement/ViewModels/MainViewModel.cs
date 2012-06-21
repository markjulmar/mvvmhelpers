using System;
using System.Windows.Input;
using JulMar.Windows.Mvvm;
using JulMar.Windows.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using JulMar.Windows.UI;

namespace ServiceReplacement.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public ICommand CalculatePi { get; private set; }

        private string _piText;
        public string PiText
        {
            get { return _piText; }
            set { _piText = value; RaisePropertyChanged(() => PiText); }
        }

        public MainViewModel()
        {
            CalculatePi = new DelegateCommand(OnCalculatePi);
        }

        private void OnCalculatePi()
        {
            IMessageVisualizer messageVisualizer = Resolve<IMessageVisualizer>();
            var result = messageVisualizer.Show("Calculating Pi",
                                                "This operation takes a long time. Are you sure you want to proceed?",
                                                new MessageVisualizerOptions(UICommand.YesNo));
            if (result == UICommand.Yes)
            {
                IDisposable waitNotify = Resolve<INotificationVisualizer>().BeginWait("Working", "Calculating Pi.. Please Wait");

                Task.Factory.StartNew(() =>
                                          {
                                              Thread.Sleep(5000);
                                              PiText = Math.PI.ToString();
                                          })
                    .ContinueWith(t => waitNotify.Dispose(), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }
    }
}
