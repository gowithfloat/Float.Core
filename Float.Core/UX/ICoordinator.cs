using System;

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
        /// An enum for the possible results of a coordinator's RequestFinish.
        /// </summary>
        public enum CoordinatorRequestFinishStatus
        {
            /// <summary>
            /// The coordinator RequestFinish finished immediately.
            /// </summary>
            FinishedImmediately,

            /// <summary>
            /// The coordinator RequestFinish is pending a finish.
            /// </summary>
            PendingFinish,

            /// <summary>
            /// The coordinator RequestFinish will not complete.
            /// </summary>
            WillNotFinish,

            /// <summary>
            /// The coordinator RequestFinish's status is unknown.
            /// </summary>
            Unknown,
        }

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
        CoordinatorRequestFinishStatus RequestFinish(EventArgs eventArgs);
    }
}
