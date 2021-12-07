using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Float.Core.Commands
{
    /// <summary>
    /// Debounces multiple simultaneous attempts to execute the same command.
    /// </summary>
    /// <remarks>Consider moving into Float.Core.</remarks>
    public class DebounceCommand : ICommand
    {
        readonly ICommand command;
        readonly int delay;
        bool debouncing;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebounceCommand"/> class.
        /// </summary>
        /// <param name="command">The command to wrap.</param>
        /// <param name="delay">The delay until another command can be executed (default is 300ms).</param>
        public DebounceCommand(ICommand command, int delay = 300)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            this.delay = delay;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebounceCommand"/> class.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        /// <param name="delay">The delay until the action can be executed again (default is 300ms).</param>
        public DebounceCommand(Action action, int delay = 300) : this(new Command(action), delay)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DebounceCommand"/> class.
        /// </summary>
        /// <param name="action">The action to perform with the given parameter.</param>
        /// <param name="delay">The delay until the action can be executed again (default is 300ms).</param>
        public DebounceCommand(Action<object> action, int delay = 300) : this(new Command(action), delay)
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
            return !debouncing && command.CanExecute(parameter);
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            // do a quick check here before bothering to call CanExecute
            if (debouncing)
            {
                return;
            }

            if (!CanExecute(parameter))
            {
                return;
            }

            debouncing = true;
            command.Execute(parameter);

            Task.Run(async () =>
            {
                await Task.Delay(delay).ConfigureAwait(false);
                debouncing = false;
            });
        }
    }
}
