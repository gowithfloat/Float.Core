using System;

namespace Float.Core.Events
{
    /// <summary>
    /// Typed event arguments.
    /// </summary>
    /// <typeparam name="T">The type of the object held in the event args.</typeparam>
    public class TypedEventArgs<T> : EventArgs
    {
        readonly T data;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypedEventArgs{T}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public TypedEventArgs(T data)
        {
            this.data = data;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The data.</value>
        public T Data => data;
    }
}
