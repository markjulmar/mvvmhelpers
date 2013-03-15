JulMar Model-View-ViewModel library for Windows 8/Metro
--------------------------------------------------------
This library is provided "as-is" with no warranty. It was developed by JulMar Technology, Inc. and is  
distributed from www.julmar.com. You are free to utilize the source code (in whole, or in part) or 
provided assembly however you desire, including creating derivative works without any compensation or requirements back
to JulMar Technology, Inc..  Any questions or comments can be directed to "mark@julmar.com"  This library was intended to be used 
as a learning and teaching aid, and I hope it has some value to each person who looks at it.

Mark Smith mark@julmar.com

Releases:

1.00
Initial revision - all basic functionality is present.
Based on Windows 8 CP

1.01
Rebuilt for Windows 8 RTM
Ported ViewModelLocator and most of MVVMHelpers 4.10
Ported Blend behavior support

1.02
Added NameScopeBinding to allow ElementName binding to attached collection items
Changed InvokeAction to pass trigger parameter if CommandParameter is null.

1.03
Fixed a bug in the ViewModelLocator when back-navigation occurs
Removed the public ViewModelLocator breaking change. Use IViewModelLocator interface instead
Added ViewModelLocatorResource for binding in XAML
Added ServiceLocatorResource for binding services in XAML
Added unit tests and new test program

1.04 9/2012
Modified ViewModelTrigger to support Action and Action<T>
Added static Designer class to port WPF code
IViewModelLocator moved to JulMar.Windows.Interfaces (instead of Core)
Added InDesignMode property to SimpleViewModel
Removed dead code from ViewModelLocator
Fixed MessageMediator to properly return true/false when targets have been collected
Changed MessageMediator back to public type so it can be used multiple times
Changed ServiceProvider implementation class from public to internal.
Moved MessageVisualizerOptions to JulMar.Windows.UI (was in interfaces)
Updated some unit tests

1.05 3/2013
Minor bug fixes
Added Flyout control and IFlyoutVisualizer
Integrated StateManager into PageNavigator and abstracted with IStateManager
Added support to locate ViewModels through attached property (still supports dictionary)
Added support to provide custom serialization on a ViewModel by implementing IViewModelStateManagement

1.06 3/2013
Fixes for PageNavigator/StateManager
Expose KnownTypes on IStateManager
Allow persistence even when objects do not participate in INavigateAware
Refactored code to share persistence on suspend + navigation.

1.07 3/2013
Fixes to reattach event handlers from EventTrigger when UIElement is unloaded/loaded multiple times.