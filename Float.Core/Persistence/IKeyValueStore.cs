// <copyright file="IKeyValueStore.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

namespace Float.Core.Persistence
{
    /// <summary>
    /// Key value data store.
    /// </summary>
    public interface IKeyValueStore
    {
        /// <summary>
        /// Put the specified key and value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <typeparam name="T">The type of value being stored.</typeparam>
        void Put<T>(string key, T value);

        /// <summary>
        /// Get the value for the specified key.
        /// </summary>
        /// <returns>The value previously stored.</returns>
        /// <param name="key">The key.</param>
        /// <typeparam name="T">The type of value to return.</typeparam>
        T Get<T>(string key);

        /// <summary>
        /// Delete the value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        void Delete(string key);
    }
}
