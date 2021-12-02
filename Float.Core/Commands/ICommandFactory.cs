// <copyright file="ICommandFactory.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System.Windows.Input;

namespace Float.Core.Commands
{
    /// <summary>
    /// A command factory creates <see cref="ICommand"/> instances to use in view models
    /// for quick binding to UI elements (such as buttons).
    /// This allows for small reusable behaviors that can be implemented on various screens
    /// in the application without worrying about injecting all the neccessary dependencies.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <returns>The command.</returns>
        ICommand CreateCommand();
    }

    /// <summary>
    /// A command factory creates <see cref="ICommand"/> instances to use in view models
    /// for quick binding to UI elements (such as buttons).
    /// This allows for small reusable behaviors that can be implemented on various screens
    /// in the application without worrying about injecting all the neccessary dependencies.
    /// </summary>
    /// <typeparam name="T">The type of parameter being passed to the command.</typeparam>
    public interface ICommandFactory<T> : ICommandFactory
    {
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <returns>The command.</returns>
        /// <param name="property">A property to use when invoking the command.</param>
        ICommand CreateCommand(T property);
    }
}
