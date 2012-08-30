using Windows.UI.Xaml;

namespace System.Windows.Behaviors
{
    /// <summary>
    /// This class allows ElementName bindings to be carried into attached properties
    /// and behaviors by placing a reference to them into a resource collection
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <Page.Resources>
    ///    <NameScopeBinding Element="{Binding ElementName=theList}" />
    /// </Page.Resources>
    /// <ListBox x:Name=theList>
    ///    <interactivity:Interaction.Triggers>
    ///       <interactivity:EventTrigger EventName="SelectionChanged">
    ///          <julmar:InvokeCommand Command="{Binding SelectCommand}" CommandParameter="{Binding Element.SelectedItem, Source={StaticResource theList}}" />
    ///       </interactivity:EventTrigger>
    ///    <interactivity:Interaction.Triggers>
    /// </ListBox>
    /// ]]>
    /// </example>
    public class NameScopeBinding : FrameworkElement
    {
        /// <summary>
        /// Backing storage for Element property
        /// </summary>
        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(
            "Element", typeof(object), typeof(NameScopeBinding), new PropertyMetadata(null));

        /// <summary>
        /// Element to bind to and make available as a resource
        /// </summary>
        public object Element
        {
            get { return GetValue(ElementProperty); }
            set { SetValue(ElementProperty, value); }
        }
    }
}
