using System;
using System.Windows.Input;

namespace Float.Core.Commands
{
    /// <summary>
    /// A convenient base implementation of a <see cref="ICommandFactory"/> that
    /// invokes a typed method when the command is invoked.
    /// </summary>
    /// <remarks>
    /// A command factory creates <see cref="ICommand"/> instances to use in view models
    /// for quick binding to UI elements (such as buttons).
    /// This allows for small reusable behaviors that can be implemented on various screens
    /// in the application without worrying about injecting all the necessary dependencies.
    /// </remarks>
    /// <typeparam name="T">The type of property passed to the command.</typeparam>
    public abstract class CommandFactory<T> : ICommandFactory<T>
    {
        /// <inheritdoc />
        public ICommand CreateCommand(T property)
        {
            return new CommandProxy(() =>
            {
                try
                {
                    Execute(property);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    HandleException(e);
                }
            });
        }

        /// <inheritdoc />
        ICommand ICommandFactory.CreateCommand()
        {
            throw new NotSupportedException("An argument must be specified");
        }

        /// <summary>
        /// Execute the command with the specified property.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected abstract void Execute(T parameter);

        /// <summary>
        /// Receives control of any exception that occurs when invoking <see cref="Execute" />.
        /// </summary>
        /// <param name="e">The exception.</param>
        protected abstract void HandleException(Exception e);

        internal class CommandProxy : ICommand
        {
            readonly Action action;

            public CommandProxy(Action action)
            {
                this.action = action ?? throw new ArgumentNullException(nameof(action));
            }

#pragma warning disable 0067
            public event EventHandler CanExecuteChanged;
#pragma warning restore 0067

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                action();
            }
        }
    }
}
