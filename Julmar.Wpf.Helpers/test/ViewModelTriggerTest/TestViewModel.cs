using System;
using JulMar.Windows.Interfaces;
using JulMar.Windows.Mvvm;

namespace ViewModelTriggerTest
{
    public class TestViewModel : SimpleViewModel
    {
        public event Action ChangeColor;
        public event Action<object> ChangeColorButton;

        public IDelegateCommand RunAction { get; set; }
        public IDelegateCommand RunAction2 { get; set; }

        public TestViewModel()
        {
            RunAction = new DelegateCommand(DoRunAction);
            RunAction2 = new DelegateCommand(DoRunAction2);
        }

        private void DoRunAction()
        {
            if (ChangeColor != null)
                ChangeColor();
        }

        private void DoRunAction2()
        {
            if (ChangeColorButton != null)
                ChangeColorButton("Yellow");
        }

    }
}