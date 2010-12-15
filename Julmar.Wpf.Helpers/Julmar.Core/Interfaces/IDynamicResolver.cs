namespace JulMar.Core.Interfaces
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

        /// <summary>
        /// This is used to resolve a set of targets once - 
        /// with this method, MEF will not hold a reference to
        /// the composed tree of objects
        /// </summary>
        void ComposeOnce(params object[] targets);

        /// <summary>
        /// Retrieves the specified exported object by type, or NULL if it doesn't exist.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <returns>Created object</returns>
        T GetExportedValue<T>();
    }
}