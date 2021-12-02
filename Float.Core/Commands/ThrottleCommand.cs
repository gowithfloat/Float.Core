// <copyright file="ThrottleCommand.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Float.Core.Commands
{
    /// <summary>
    /// Delays invokation of a command until requests to perform the command stop (e.g. waiting until the last character is typed in a search query).
    /// </summary>
    /// <remarks>Consider moving into Float.Core.</remarks>
    public class ThrottleCommand : ICommand
    {
        readonly ICommand command;
        readonly int delay;
        CancellationTokenSource taskCancellation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleCommand"/> class.
        /// </summary>
        /// <param name="command">The command to wrap.</param>
        /// <param name="delay">The time to wait before executing the command in milliseconds (default is 500ms).</param>
        public ThrottleCommand(ICommand command, int delay = 500)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            this.delay = delay;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleCommand"/> class.
        /// </summary>
        /// <param name="action">The action to wrap in a command.</param>
        /// <param name="delay">The time to wait before executing the command (default is 500ms).</param>
        public ThrottleCommand(Action action, int delay = 500) : this(new Command(action ?? throw new ArgumentNullException(nameof(action))), delay)
        {
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged
        {
            add => command.CanExecuteChanged += value;
            remove => command.CanExecuteChanged -= value;
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return command.CanExecute(parameter);
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            taskCancellation?.Cancel();

            var cancellationTokenSource = new CancellationTokenSource();
            taskCancellation = cancellationTokenSource;

            Task.Run(async () =>
            {
                await Task.Delay(delay).ConfigureAwait(false);

                if (cancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                command.Execute(parameter);
                taskCancellation = null;
            });
        }
    }
}
