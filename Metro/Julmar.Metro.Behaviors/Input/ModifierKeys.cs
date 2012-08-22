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
        None = 0,
        Control = 1,
        Menu = 2,
        Shift = 4,
        Windows = 8,
    }
}