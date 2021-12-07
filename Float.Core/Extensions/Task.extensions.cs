using System;
using System.Threading.Tasks;

namespace Float.Core.Extensions
{
    /// <summary>
    /// Extensions on Task objects.
    /// </summary>
    public static class TaskExtension
    {
        /// <summary>
        /// Set a continuation action to be invoked if this task completes successfully.
        /// </summary>
        /// <returns>The task object. Useful for chaining.</returns>
        /// <param name="task">This task object.</param>
        /// <param name="continuationAction">Continuation action to handle success.</param>
        /// <typeparam name="T">The result type of the task.</typeparam>
        public static Task<T> OnSuccess<T>(this Task<T> task, Action<Task<T>> continuationAction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            task.ContinueWith(continuationAction, default, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent, TaskScheduler.Current);
            return task;
        }

        /// <summary>
        /// Set a continuation action to be invoked if this task completes successfully.
        /// </summary>
        /// <returns>The task object. Useful for chaining.</returns>
        /// <param name="task">This task object.</param>
        /// <param name="continuationAction">Continuation action to handle success.</param>
        public static Task OnSuccess(this Task task, Action<Task> continuationAction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            task.ContinueWith(continuationAction, default, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent, TaskScheduler.Current);
            return task;
        }

        /// <summary>
        /// Set a continuation action to be invoked if this task experiences a fault.
        /// </summary>
        /// <returns>The task object. Useful for chaining.</returns>
        /// <param name="task">This task object.</param>
        /// <param name="continuationAction">Continuation action to handle fault.</param>
        /// <typeparam name="T">The result type of the task.</typeparam>
        public static Task<T> OnFailure<T>(this Task<T> task, Action<Task<T>> continuationAction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            task.ContinueWith(
                result =>
                {
                    // handle aggregate exception to prevent a crash
                    result.Exception.Handle(exception =>
                    {
                        return true;
                    });

                    continuationAction.Invoke(result);
                },
                default,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent,
                TaskScheduler.Current);

            return task;
        }

        /// <summary>
        /// Set a continuation action to be invoked if this task experiences a fault.
        /// </summary>
        /// <returns>The task object. Useful for chaining.</returns>
        /// <param name="task">This task object.</param>
        /// <param name="continuationAction">Continuation action to handle fault.</param>
        public static Task OnFailure(this Task task, Action<Task> continuationAction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            task.ContinueWith(
                result =>
                {
                    // handle aggregate exception to prevent a crash
                    result.Exception.Handle(exception =>
                    {
                        return true;
                    });

                    continuationAction.Invoke(result);
                },
                default,
                TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent,
                TaskScheduler.Current);

            return task;
        }

        /// <summary>
        /// Called only when the task completes with a true value.
        /// Note that unlike OnFailure, this does no error handling.
        /// </summary>
        /// <returns>The task object. Useful for chaining.</returns>
        /// <param name="task">This task object.</param>
        /// <param name="continuationAction">Continuation action to perform.</param>
        public static Task<bool> OnTrue(this Task<bool> task, Action continuationAction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            task.ContinueWith(
                result =>
                {
                    if (task.Result)
                    {
                        continuationAction.Invoke();
                    }
                },
                TaskScheduler.Current);

            return task;
        }

        /// <summary>
        /// Called only when the task completes with a false value.
        /// Note that unlike OnFailure, this does no error handling.
        /// </summary>
        /// <returns>The task object. Useful for chaining.</returns>
        /// <param name="task">This task object.</param>
        /// <param name="continuationAction">Continuation action to perform.</param>
        public static Task<bool> OnFalse(this Task<bool> task, Action continuationAction)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (continuationAction == null)
            {
                throw new ArgumentNullException(nameof(continuationAction));
            }

            task.ContinueWith(
                result =>
                {
                    if (!task.Result)
                    {
                        continuationAction.Invoke();
                    }
                }, TaskScheduler.Current);

            return task;
        }
    }
}
