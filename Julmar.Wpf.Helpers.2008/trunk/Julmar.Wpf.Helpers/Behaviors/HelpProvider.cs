using System.Windows;
using System.Windows.Input;

namespace JulMar.Windows.Behaviors
{
    // This class and code was borrowed from http://peteohanlon.wordpress.com/2009/05/01/easy-help-with-wpf/
    // and from Mike Hilbergs similar post - http://blogs.msdn.com/mikehillberg/archive/2007/07/26/a-context-sensitive-help-provider-in-wpf.aspx
    // Thanks to both Pete and Mike for the code and inspiration.
    /// <summary>
    /// This class provides the ability to easily attach Help functionality
    /// to Framework elements. To use it, you need to
    /// add a reference to the HelpProvider in your XAML. The FilenameProperty
    /// is used to specify the name of the helpfile, and the KeywordProperty specifies
    /// the keyword to be used with the search.
    /// </summary>
    /// <remarks>
    /// The FilenameProperty can be at a higher level of the visual tree than
    /// the KeywordProperty, so you don't need to set the filename each time.
    /// </remarks>
    /// <example>
    /// <![CDATA[
    /// <Window xmlns:help="..."
    ///         julmar:HelpProviduer.Filename="MyHelpfile.chm">
    ///   ...
    ///    <TextBox julmar:HelpProvider.Keyword="MyKeyword" Grid.Row="0" Text="Keyword based search" />
    /// </Window>
    /// ]]>
    /// </example>
    public static class HelpProvider
    {
        /// <summary>
        /// Initialize a new instance of <see cref="HelpProvider"/>.
        /// </summary>
        static HelpProvider()
        {
            // Handle all instances of the ApplicationCommands.Help command
            CommandManager.RegisterClassCommandBinding(typeof(FrameworkElement), new CommandBinding(ApplicationCommands.Help, Executed, CanExecute));
        }

        /// <summary>
        /// Filename Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty FilenameProperty = DependencyProperty.RegisterAttached(
                            "Filename", typeof(string), typeof(HelpProvider),
                            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits));

        /// <summary>
        /// Gets the Filename property.
        /// </summary>
        public static string GetFilename(DependencyObject d)
        {
            return (string)d.GetValue(FilenameProperty);
        }

        /// <summary>
        /// Sets the Filename property.
        /// </summary>
        public static void SetFilename(DependencyObject d, string value)
        {
            d.SetValue(FilenameProperty, value);
        }

        /// <summary>
        /// Keyword Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty KeywordProperty = DependencyProperty.RegisterAttached(
                            "Keyword", typeof(string), typeof(HelpProvider));

        /// <summary>
        /// Gets the Keyword property.
        /// </summary>
        public static string GetKeyword(DependencyObject d)
        {
            return (string)d.GetValue(KeywordProperty);
        }

        /// <summary>
        /// Sets the Keyword property.
        /// </summary>
        public static void SetKeyword(DependencyObject d, string value)
        {
            d.SetValue(KeywordProperty, value);
        }

        /// <summary>
        /// Handler for the ApplicationCommands.Help.CanExecute method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs args)
        {
            var fe = sender as FrameworkElement;
            if (fe != null)
                args.CanExecute = !string.IsNullOrEmpty(GetFilename(fe));
        }

        /// <summary>
        /// Handler for the ApplicationCommands.Help.Executed method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Executed(object sender, ExecutedRoutedEventArgs args)
        {
            var parent = args.OriginalSource as DependencyObject;
            string keyword = GetKeyword(parent);
            if (!string.IsNullOrEmpty(keyword))
            {
                System.Windows.Forms.Help.ShowHelp(null, GetFilename(parent), keyword);
            }
            else
            {
                System.Windows.Forms.Help.ShowHelp(null, GetFilename(parent));
            }
        }
    }
}
