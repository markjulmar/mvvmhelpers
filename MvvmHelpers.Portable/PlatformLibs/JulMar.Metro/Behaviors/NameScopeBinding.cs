using Windows.UI.Xaml;

namespace JulMar.Behaviors
{
    /// <summary>
    /// This class allows ElementName bindings to be carried into attached properties
    /// and behaviors by placing a reference to them into a resource collection
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// <Page.Resources>
    ///    <NameScopeBinding Source="{Binding ElementName=theList}" />
    /// </Page.Resources>
    /// <ListBox x:Name=theList>
    ///    <interactivity:Interaction.Triggers>
    ///       <interactivity:EventTrigger EventName="SelectionChanged">
    ///          <julmar:InvokeCommand Command="{Binding SelectCommand}" CommandParameter="{Binding Source.SelectedItem, Source={StaticResource theList}}" />
    ///       </interactivity:EventTrigger>
    ///    <interactivity:Interaction.Triggers>
    /// </ListBox>
    /// ]]>
    /// </example>
    public sealed class NameScopeBinding : DependencyObject
    {
        /// <summary>
        /// Backing storage for Element property
        /// </summary>
        public static readonly DependencyProperty SourceProperty = 
            DependencyProperty.Register("Source", typeof(object), typeof(NameScopeBinding), new PropertyMetadata(null));

        /// <summary>
        /// Source to bind to and make available as a resource
        /// </summary>
        public object Source
        {
            get { return this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }
    }
}
