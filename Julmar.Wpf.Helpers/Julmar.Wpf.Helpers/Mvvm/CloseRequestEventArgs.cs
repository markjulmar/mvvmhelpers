using System;

namespace JulMar.Windows.Mvvm
{
    /// <summary>
    /// This is used to send result parameters to a CloseRequest
    /// </summary>
    public class CloseRequestEventArgs : EventArgs
    {
        ///<summary>
        /// Final result for ShowDialog
        ///</summary>
        public bool? Result { get; private set; }
        internal CloseRequestEventArgs(bool? result)
        {
            Result = result;
        }
    }
}