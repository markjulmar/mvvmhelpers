using System.Windows.Controls;

namespace JulMar.Windows.Internal
{
    /// <summary>
    /// This class is passed around from drop source to target
    /// </summary>
    class DragInfo
    {
        public object Data { get; set; }
        public ItemsControl Source { get; set; }
        public bool AllowOnlySelf { get; set; }
    }
}