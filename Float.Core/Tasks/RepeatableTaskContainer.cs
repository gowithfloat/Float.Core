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
    public class RepeatableTaskContainer
    {
        readonly Func<Task> taskFactory;
        readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        Task runningTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatableTaskContainer"/> class.
        /// </summary>
        /// <param name="taskFactory">A function to call asynchronously.</param>
        public RepeatableTaskContainer(Func<Task> taskFactory)
        {
            this.taskFactory = taskFactory ?? throw new ArgumentNullException(nameof(taskFactory));
        }

        /// <summary>
        /// Gets a value indicating whether the task is currently running.
        /// </summary>
        /// <value><c>true</c> if the task is currently running.</value>
        public bool IsRunning => runningTask != null;

        /// <summary>
        /// Run the given function asynchronously and return the result to awaiters.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task Run()
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
                await runningTask.ConfigureAwait(false);
            }
            finally
            {
                if (runningTask is Task task)
                {
                    if (task.IsCanceled == true || task.IsCompleted == true || task.IsFaulted == true)
                    {
                        task.Dispose();
                    }
                    else
                    {
                        Console.WriteLine($"Unable to dispose of running task in state: {runningTask.Status}");
                    }

                    runningTask = null;
                }
            }
        }
    }
}
