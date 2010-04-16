namespace JulMar.Windows.Interfaces
{
    /// <summary>
    /// This interface is exposed through the service provider to get to 
    /// the resolver (MEF).
    /// </summary>
    public interface IDynamicResolver
    {
        /// <summary>
        /// Used to resolve a set of targets.
        /// </summary>
        void Compose(params object[] targets);
    }
}