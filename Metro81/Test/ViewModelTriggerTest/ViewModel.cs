using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace MVVMTest
{
    public class Payload
    {
        public int Value;
    }

    public class ViewModel : JulMar.Windows.Mvvm.ViewModel
    {
        private string _result;
        public event Action<Payload> SomeEvent;

        public string Result
        {
            get { return _result; }
            set { SetPropertyValue(ref _result, value); }
        }

        public IDelegateCommand RunTrigger { get; private set; }

        public ViewModel()
        {
            RunTrigger = new DelegateCommand(() => OnSomeEvent(new Payload() { Value = 10 }));
        }

        public void OnSomeEvent(Payload payload)
        {
            SomeEvent(payload);
        }

        public void DummyMethod()
        {
            Result = "Dummy Method called!";
            return;
        }
    }
}
