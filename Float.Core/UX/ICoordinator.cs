using System;
using System.Threading.Tasks;

namespace Float.Core.UX
{
    /// <summary>
    /// Describes a coordinator that manages some related group of functions that can be started and stopped.
    /// </summary>
    public interface ICoordinator
    {
        /// <summary>
        /// Implementing classes should send this event when they are started.
        /// </summary>
        event EventHandler Started;

        /// <summary>
        /// Implementing classes should send this event when they are finished.
        /// All coordinators are responsible for finishing themselves.
        /// </summary>
        event EventHandler<EventArgs> Finished;

        /// <summary>
        /// Implementing classes should use this to start this coordinator.
        /// </summary>
        /// <param name="navigationContext">The navigation context for this coordinator.</param>
        void Start(INavigationContext navigationContext);

        /// <summary>
        /// Requests that a coordinator will finish.
        /// </summary>
        /// <param name="eventArgs">The event args.</param>
        /// <returns>A task, with a boolean indicating whether this did finish.</returns>
        Task<bool> RequestFinish(EventArgs eventArgs);
    }
}
