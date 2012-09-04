using System;

namespace JulMar.Windows.Interactivity.Input
{
    /// <summary>
    /// This mirrors the VirtualKeyModifiers, but is based on int vs. uint
    /// so it can be created in XAML.
    /// </summary>
    [Flags]
    public enum ModifierKeys
    {
        /// <summary>
        /// No modifier
        /// </summary>
        None = 0,
        /// <summary>
        /// Ctrl key
        /// </summary>
        Control = 1,
        /// <summary>
        /// ALT key
        /// </summary>
        Menu = 2,
        /// <summary>
        /// Shift key
        /// </summary>
        Shift = 4,
        /// <summary>
        /// Windows key
        /// </summary>
        Windows = 8,
    }
}