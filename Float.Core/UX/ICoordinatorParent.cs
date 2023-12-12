using System;
using System.Threading.Tasks;

namespace Float.Core.UX
{
    /// <summary>
    /// Defines an object that has children coordinators.
    /// </summary>
    public interface ICoordinatorParent
    {
        /// <summary>
        /// Implementing classes should use this to add a coordinator to its internal collection of children.
        /// </summary>
        /// <param name="coordinator">The coordinator to add.</param>
        void AddChild(ICoordinator coordinator);

        /// <summary>
        /// Implementing classes should use this to remove a coordinator from its internal collection of children.
        /// </summary>
        /// <param name="coordinator">The coordinator to remove.</param>
        void RemoveChild(ICoordinator coordinator);
    }
}
