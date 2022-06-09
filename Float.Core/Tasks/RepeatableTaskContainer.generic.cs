using System;
using System.Threading;
using System.Threading.Tasks;

namespace Float.Core.Tasks
{
    /// <summary>
    /// Contains a task that can be run repeatedly.
    /// Subsequent calls to <c>Run()</c> before the task finishes will await the same task.
    /// After the task completes, the next call to <c>Run()</c> will create a new task.
    /// </summary>
    /// <typeparam name="TResult">The type returned from the given factory method.</typeparam>
    public class RepeatableTaskContainer<TResult>
    {
        readonly Func<Task<TResult>> taskFactory;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        Task<TResult> runningTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatableTaskContainer{T}"/> class.
        /// </summary>
        /// <param name="taskFactory">A function to call asynchronously.</param>
        public RepeatableTaskContainer(Func<Task<TResult>> taskFactory)
        {
            this.taskFactory = taskFactory ?? throw new ArgumentNullException(nameof(taskFactory));
        }

        /// <summary>
        /// Run the given function asynchronously and return the result to awaiters.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<TResult> Run()
        {
            // Ensure we only ever have one running instance of the task.
            semaphore.Wait();
            if (runningTask == null)
            {
                runningTask = taskFactory();
            }

            semaphore.Release();

            try
            {
                return await runningTask.ConfigureAwait(false);
            }
            finally
            {
                runningTask?.Dispose();
                runningTask = null;
            }
        }
    }
}
