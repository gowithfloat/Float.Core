// <copyright file="CommandFactoryRegistry.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Collections.Generic;

namespace Float.Core.Commands
{
    /// <summary>
    /// Register command factory instances to be used throughout the application.
    /// </summary>
    /// <remarks>
    /// Any class may register a <see cref="ICommandFactory"/>
    /// (however it is recommended that this occurs in a <see cref="UX.ICoordinator"/>).
    /// Only view models may retrieve a registered <see cref="ICommandFactory"/>.
    /// </remarks>
    public static class CommandFactoryRegistry
    {
        static readonly Dictionary<Type, ICommandFactory> Registry = new ();

        /// <summary>
        /// Register the specified implementation for a given command factory.
        /// </summary>
        /// <param name="impl">The implementation to register.</param>
        /// <typeparam name="TCommandFactory">The type of command factory.</typeparam>
        public static void Register<TCommandFactory>(TCommandFactory impl) where TCommandFactory : class, ICommandFactory
        {
            if (impl == null)
            {
                throw new ArgumentNullException(nameof(impl));
            }

            var key = typeof(TCommandFactory);

            if (Registry.ContainsKey(key))
            {
                Registry.Remove(key);
            }

            Registry.Add(key, impl);
        }

        /// <summary>
        /// Unregister the an implementation for a given command factory.
        /// </summary>
        /// <typeparam name="TCommandFactory">The type of command factory.</typeparam>
        public static void Unregister<TCommandFactory>() where TCommandFactory : class, ICommandFactory
        {
            Registry.Remove(typeof(TCommandFactory));
        }

        /// <summary>
        /// Retrieves a registered command factory.
        /// </summary>
        /// <remarks>
        /// This method is internal because commands are intended only to be used in view models.
        /// To help enforce this design decision, only view models may retrieve a command factory instance.
        /// </remarks>
        /// <returns>The command factory.</returns>
        /// <typeparam name="TCommandFactory">The type of command factory.</typeparam>
        internal static TCommandFactory Get<TCommandFactory>() where TCommandFactory : class, ICommandFactory
        {
            var key = typeof(TCommandFactory);

            if (Registry.ContainsKey(key) == false)
            {
                throw new CommandFactoryRegistryException($"No CommandFactory has been registered for {key}");
            }

            return Registry[key] as TCommandFactory;
        }
    }
}
