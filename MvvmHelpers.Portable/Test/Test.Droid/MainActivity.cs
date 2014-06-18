using Android.App;
using Android.Widget;
using Android.OS;

using JulMar.Mvvm;

using Test.Shared.ViewModels;

namespace Test.Droid
{
    [Activity(Label = "Test.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            TextView label = FindViewById<TextView>(Resource.Id.TheLabel);

            // Do the bindings.
            TestViewModel vm = new TestViewModel();

            BindingContext.Create(vm)
                .Add(s => s.TotalClicks, label, l => l.Text)
                .Add(s => s.TotalClicks, s => button.Text = "Total Clicks is now " + s.TotalClicks);

            CommandBinder.Create().Add(button, "Click", vm.IncrementCounter);

        }
    }
}


