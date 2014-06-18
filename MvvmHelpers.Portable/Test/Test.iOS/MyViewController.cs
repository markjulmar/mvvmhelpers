using JulMar.Mvvm;

using MonoTouch.UIKit;
using System.Drawing;

using Test.Shared.ViewModels;

namespace Test.iOS
{
    public class MyViewController : UIViewController
    {
        UIButton button;
        float buttonWidth = 200;
        float buttonHeight = 50;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            View.Frame = UIScreen.MainScreen.Bounds;
            View.BackgroundColor = UIColor.White;
            View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            UILabel label = new UILabel(new RectangleF(0, 30, View.Frame.Width, 50));
            View.AddSubview(label);

            button = UIButton.FromType(UIButtonType.RoundedRect);

            button.Frame = new RectangleF(
                View.Frame.Width / 2 - buttonWidth / 2,
                View.Frame.Height / 2 - buttonHeight / 2,
                buttonWidth,
                buttonHeight);

            button.SetTitle("Click me", UIControlState.Normal);
            View.AddSubview(button);

            // Do the bindings.
            TestViewModel vm = new TestViewModel();
            
            BindingContext.Create(vm)
                .Add(s => s.TotalClicks, label, l => l.Text)
                .Add(s => s.TotalClicks, s => button.SetTitle("Total Clicks is now " + s.TotalClicks, UIControlState.Normal));
            
            CommandBinder.Create()
                .Add(button, "TouchUpInside", vm.IncrementCounter, 
                    stateChanged: tf => button.Enabled = tf);
        }

    }
}

