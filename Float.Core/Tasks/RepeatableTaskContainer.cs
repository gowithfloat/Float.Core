﻿// <copyright file="RepeatableTaskContainer.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
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
        /// Run the given function asynchronously and return the result to awaiters.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task Run()
        {
            if (runningTask == null)
            {
                runningTask = taskFactory();
            }

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