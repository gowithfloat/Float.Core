namespace Float.Core.Definitions
{
    /// <summary>
    /// Defines an object with a name. The name does not necessarily need to be unique.
    /// </summary>
    public interface INamed
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name for this object.</value>
        string Name { get; }
    }
}
