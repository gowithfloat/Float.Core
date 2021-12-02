// <copyright file="MultiCommand.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Float.Core.Commands
{
    /// <summary>
    /// A command that is comprised of multiple commands.
    /// </summary>
    public class MultiCommand : ICommand
    {
        readonly IEnumerable<ICommand> commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiCommand"/> class.
        /// </summary>
        /// <param name="commands">The commands to group together under this command.</param>
        public MultiCommand(IEnumerable<ICommand> commands)
        {
            this.commands = commands ?? throw new ArgumentNullException(nameof(commands));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiCommand"/> class.
        /// </summary>
        /// <param name="commands">The commands to group together under this command.</param>
        public MultiCommand(params ICommand[] commands) : this(commands as IEnumerable<ICommand>)
        {
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged
        {
            add
            {
                foreach (var command in commands)
                {
                    command.CanExecuteChanged += value;
                }
            }

            remove
            {
                foreach (var command in commands)
                {
                    command.CanExecuteChanged -= value;
                }
            }
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return commands.All(c => c.CanExecute(parameter));
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            foreach (var command in commands)
            {
                command.Execute(parameter);
            }
        }
    }
}
