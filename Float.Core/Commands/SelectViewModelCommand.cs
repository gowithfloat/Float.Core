using System;
using System.Windows.Input;
using Float.Core.ViewModels;
#if NETSTANDARD
using Xamarin.Forms;
#else
using Microsoft.Maui;
using Microsoft.Maui.Controls;
#endif

namespace Float.Core.Commands
{
    /// <summary>
    /// A command that passes a view model's underlying model to the receiver.
    /// </summary>
    /// <typeparam name="T">The type of the model underlying a view model.</typeparam>
    public class SelectViewModelCommand<T> : ICommand
    {
        readonly Command<T> command;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectViewModelCommand{T}"/> class.
        /// </summary>s
        /// <param name="command">A command to invoke.</param>
        public SelectViewModelCommand(Command<T> command)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectViewModelCommand{T}"/> class.
        /// </summary>
        /// <param name="action">An action to invoke.</param>
        public SelectViewModelCommand(Action<T> action) : this(new Command<T>(action ?? throw new ArgumentNullException(nameof(action))))
        {
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged
        {
            add
            {
                command.CanExecuteChanged += value;
            }

            remove
            {
                command.CanExecuteChanged -= value;
            }
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return parameter is ViewModel<T> && command.CanExecute(parameter);
        }

        /// <summary>
        /// Execute this command.
        /// </summary>
        /// <param name="parameter">A view model.</param>
        public void Execute(ViewModel<T> parameter)
        {
            if (parameter == default)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (!CanExecute(parameter))
            {
                return;
            }

            command.Execute(parameter.UnderlyingModel);
        }

        /// <inheritdoc />
        void ICommand.Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            if (parameter is ViewModel<T> viewModel)
            {
                Execute(viewModel);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}
