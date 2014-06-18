using JulMar.Core;
using JulMar.Interfaces;
using JulMar.Mvvm;
using Test.Shared.ViewModels;

[assembly: ExportViewModel("TestVm", typeof(TestViewModel))]

namespace Test.Shared.ViewModels
{
    public class TestViewModel : ViewModel
    {
        public bool CounterEnabled
        {
            get
            {
                return this.counterEnabled;
            }
            set
            {
                if (SetPropertyValue(ref this.counterEnabled, value))
                {
                    IncrementCounter.RaiseCanExecuteChanged();
                }
            }
        }

        private int totalClicks;

        private bool counterEnabled;

        public int TotalClicks
        {
            get
            {
                return this.totalClicks;
            }
            set
            {
                this.SetPropertyValue(ref this.totalClicks, value);
            }
        }

        public IDelegateCommand IncrementCounter { get; private set; }

        public TestViewModel()
        {
            this.TotalClicks = 0;
            this.IncrementCounter = new DelegateCommand(() => this.TotalClicks++, () => CounterEnabled);
        }
    }
}