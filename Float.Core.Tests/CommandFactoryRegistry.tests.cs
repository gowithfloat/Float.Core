// <copyright file="CommandFactoryRegistry.tests.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Windows.Input;
using Float.Core.Commands;
using Xunit;

namespace Float.Core.Tests
{
    public class CommandFactoryRegistryTests
    {
        /// <summary>
        /// When the command factory registry already has an implementation for a type,
        /// replace the existing implementation with the new one when a caller reregisters.
        /// </summary>
        [Fact]
        public void ReregisteringCommandFactoryShouldReplacePreviousEntry()
        {
            var commandFactory = new StubCommandFactory();
            CommandFactoryRegistry.Register(commandFactory);
            Assert.Equal(commandFactory, CommandFactoryRegistry.Get<StubCommandFactory>());

            var newCommandFactory = new StubCommandFactory();
            CommandFactoryRegistry.Register(newCommandFactory);
            Assert.Equal(newCommandFactory, CommandFactoryRegistry.Get<StubCommandFactory>());
            Assert.NotEqual(commandFactory, CommandFactoryRegistry.Get<StubCommandFactory>());
        }
    }

    class StubCommandFactory : ICommandFactory
    {
        public ICommand CreateCommand()
        {
            throw new NotImplementedException();
        }
    }

}
