using System;
using System.Collections.Generic;
using Float.Core.Extensions;

namespace Float.Core.UX
{
    /// <summary>
    /// Default implementation of ICoordinator and ICoordinatorParent.
    /// This can not be used directly; instead, it should be subclassed.
    /// </summary>
    public abstract class CoordinatorParent : Coordinator, ICoordinatorParent
    {
        /// <summary>
        /// The child coordinator list.
        /// </summary>
        readonly List<ICoordinator> childCoordinators = new ();

        /// <summary>
        /// The event args that have been used as we are waiting to finish all the children before closing the parent.
        /// </summary>
        EventArgs waitingToFinishEventArgs = null;

        /// <summary>
        /// Gets a value indicating whether this <see cref="CoordinatorParent"/> has children.
        /// </summary>
        /// <value><c>true</c> if has children; otherwise, <c>false</c>.</value>
        protected bool HasChildren => childCoordinators.Count > 0;

        /// <summary>
        /// Gets the list of child coordinators.
        /// </summary>
        /// <value>An immutable copy of the list of child coordinators.</value>
        protected IReadOnlyList<ICoordinator> ChildCoordinators => childCoordinators;

        /// <summary>
        /// Adds a coordinator as a child and starts it.
        /// </summary>
        /// <param name="coordinator">The new coordinator.</param>
        public void StartChildCoordinator(ICoordinator coordinator)
        {
            if (coordinator == null)
            {
                throw new ArgumentNullException(nameof(coordinator));
            }

            AddChild(coordinator);
            coordinator.Start(NavigationContext);
        }

        /// <summary>
        /// Adds a child to this coordinator.
        /// The child will be started when this coordinator is started, and finished when this coordinator is finished.
        /// Trying to add a child to this coordinator that this coordinator already owns will cause an exception.
        /// </summary>
        /// <param name="coordinator">The coordinator to add.</param>
        public virtual void AddChild(ICoordinator coordinator)
        {
            if (coordinator == null)
            {
                throw new ArgumentNullException(nameof(coordinator));
            }

            // todo: this may benefit from ContainsSubclassOf
            if (childCoordinators.Contains(coordinator))
            {
                throw new CoordinatorException($"Tried to add child coordinator of type {coordinator.GetType()} more than once: {this}");
            }

            // ensure that our start/finish handlers get notified of those events
            coordinator.Started += HandleChildStart;
            coordinator.Finished += HandleChildFinish;

            childCoordinators.Add(coordinator);
        }

        /// <summary>
        /// Removes a child from this coordinator.
        /// Trying to remove a coordinator that this coordinator does not own will cause an exception.
        /// </summary>
        /// <param name="coordinator">The coordinator to remove.</param>
        public virtual void RemoveChild(ICoordinator coordinator)
        {
            if (coordinator == null)
            {
                throw new ArgumentNullException(nameof(coordinator));
            }

            // TODO: maybe this function shouldn't exist? children should probably only be removed when they are finished
            if (childCoordinators.Contains(coordinator))
            {
                coordinator.Started -= HandleChildStart;
                coordinator.Finished -= HandleChildFinish;
                var success = childCoordinators.Remove(coordinator);

                if (!success)
                {
                    throw new CoordinatorException("Failed to remove child coordinator.");
                }
            }
            else
            {
                throw new CoordinatorException("Tried to remove a child coordinator that this coordinator does not own.");
            }
        }

        /// <inheritdoc />
        public virtual void FinishFamily(EventArgs args)
        {
            waitingToFinishEventArgs = args;
            NavigationContext?.DismissPageAsync(false);
            NavigationContext?.Reset(false);
        }

        /// <summary>
        /// Use to determine if this coordinator already contains a certain type of child coordinator.
        /// </summary>
        /// <typeparam name="T">The type of coordinator for which to check.</typeparam>
        /// <returns><c>true</c> if this coordinator contains a child of the specified type, <c>false</c> otherwise.</returns>
        public bool HasChild<T>() where T : ICoordinator
        {
            return childCoordinators.ContainsInstanceOf<T>();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="Coordinator"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current <see cref="Coordinator"/>.</returns>
        public override string ToString()
        {
            return $"[{GetType()}, Children: {string.Join(",", childCoordinators)}]";
        }

        /// <inheritdoc />
        protected override void Finish(EventArgs args)
        {
            if (HasChildren)
            {
                throw new CoordinatorException($"Cannot finish coordinator until all children have finished: {this}");
            }

            base.Finish(args);
        }

        /// <summary>
        /// Called when a child coordinator is started.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to this event.</param>
        protected virtual void HandleChildStart(object sender, EventArgs args)
        {
        }

        /// <summary>
        /// Called when a child coordinator is finished.
        /// By default, will remove the child.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="args">Arguments related to this event.</param>
        protected virtual void HandleChildFinish(object sender, EventArgs args)
        {
            if (sender is ICoordinator child)
            {
                RemoveChild(child);
            }

            if (!HasChildren && waitingToFinishEventArgs != null)
            {
                Finish(waitingToFinishEventArgs);
                waitingToFinishEventArgs = null;
            }
        }
    }
}
