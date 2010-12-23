JulMar Model-View-ViewModel library
--------------------------------------------------------
This library is provided "as-is" with no warranty. It was developed by JulMar Technology, Inc. and is  
distributed from www.julmar.com. You are free to utilize the source code (in whole, or in part) or 
provided assembly however you desire, including creating derivative works without any compensation or requirements back
to JulMar Technology, Inc..  Any questions or comments can be directed to "mark@julmar.com"  This libary was intended to be used 
as a learning and teaching aid, and I hope it has some value to each person who looks at it.

Mark Smith 5/2009 mark@julmar.com

Releases:

1.0: 
Initial revision - all basic functionality is present.

1.01: rolled in a bunch of fixes from internal library -- 6/09
Fixed a bug in the event commander which was causing multiple invocations in some cases.
Added support for ShowDialog return results.
Added HelpProvider behavior to support invoking Windows Help
Removed automatic mediator registration from ViewModel - unnecessary perf hit when not using service mediator - you must call RegisterWithMessageMediator() deliberately now.
Added SystemInfo class to retrieve Windows version - can detect difference between W2K8 SP2 and Windows 7 (Environment.OSVersion does not appear to do so).
Added LinearGradientMarkupExtension for easy gradients
Added ENTER/ESC/Fx/CTRL support to NumericTextBoxBehavior.
Added DelegatingCommand<T>
Added ObservableDictionary<K,V>

1.02: additional support 7/09
Added ScrollingPreviewService
Added CommandParameter and Command to EventCommander event arguments
Added Past/Drop conversion to NumericTextBoxBehavior
Split out EditingViewModel into implementation class to make it easier to provide other forms of edit support.
Added PropertyObserver<T> from Josh Smith

1.03: Added new Behaviors assembly  7/30
Added dependency to System.Windows.Interactivity.dll
Ported over existing attached behaviors to Blend style behaviors [BREAKING CHANGE]
Added WatermarkedTextBoxBehavior
Added InvokeCommand action to bind to VM ICommands
Added CommandEventTrigger to support EventCommander from Blend

1.04: 9/15
Added Designer.InDesignMode static property to test design surface
Added test for null conditions in VisualTreeHelper extensions
Moved ValidatingViewModel into proper namespace
Added clipboard paste support to the WatermarkedTextBoxBehavior
Changed MTObservableCollection<T> to support true multi-threaded access based on discussion on WPFDisciples list.

1.05 11/1
Added support for VS2010 Beta 2
Removed ViewModel.OnPropertiesChanged - merged into ViewModel.OnPropertyChanged [BREAKING CHANGE]
Added generic support to MessageMediator
Added BindingTrigger to trigger of ViewModel binding values in Blend
Added CloseWindowBehavior to auto-close a dialog or modaless form without code behind
Added SelectTextOnFocusBehavior to select all text in a TextBox when it receives focus
Added ViewportSynchronizerBehavior to track the ViewPort of a ScrollViewer and bind it to ViewModel properties.
Added Dispatcher transition for RaiseClose and RaiseActivate from VM for cross-thread invocation
Added MultiConverter to aggregate value converters together.
Added ScrollingPreviewBehavior and ScrollBarPreviewBehaviors (replaced ScrollingPreviewService)
Added DeferredScrollBehavior

1.06 4/2010
Added new behaviors and converters.  Synched with private library

2.00 4/2010
First full targeted .NET 4.0 release
Added MEF support to locate services using [ExportServiceProvider]
Added MEF support to locate ViewModels using [ExportViewModel]
Added MEF support to locate Views using [ExportUIVisualization]
Cleanup of library to target both .NET 3.5 and .NET 4.0

3.00 6/2010
Added undo/redo framework
Added CollectionObserver
Updated PropertyObserver to allow global property notifications
Added DataGridRowDragBehavior
Added ItemsControl drag/drop behavior

3.5 7/2010
Split out Julmar.Core to contain non-WPF specific types.
Moved internal ThreadedCollection into Core
Added ReaderWriterLockSlim extensions and ObjectLock extensions
Added DataGridRowIndexBehavior

4.00 8/2010
Added ObjectCloner
Added SelectedItemsCollectionSynchronizer

4.01 12/2010
Official build from rcat.codeplex.com changes
Added attached event support to EventCommander
Corrected issue with MessageMediator identifying interface targets from an implementation message
Added new constructor to ViewModel to avoid MEF registration for 3.5 memory leak issue (MEF)
Added Swap and Move extensions to IList collections
