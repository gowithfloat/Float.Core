// <copyright file="ISecureStore.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System.Threading.Tasks;

namespace Float.Core.Persistence
{
    /// <summary>
    /// Interface describing securing storing strings in a key-value store.
    /// </summary>
    public interface ISecureStore
    {
        /// <summary>
        /// Save the string for a given key.
        /// </summary>
        /// <returns>True if the save was successful, false otherwise.</returns>
        /// <param name="key">The key for the key-value store.</param>
        /// <param name="str">The string to save.</param>
        bool Put(string key, string str);

        /// <summary>
        /// Get the string for a given key.
        /// </summary>
        /// <returns>The string for the given key.</returns>
        /// <param name="key">The key for the key-value store.</param>
        string Get(string key);

        /// <summary>
        /// Get the string for a given key.
        /// </summary>
        /// <returns>The string for the given key.</returns>
        /// <param name="key">The key for the key-value store.</param>
        Task<string> GetAsync(string key);

        /// <summary>
        /// Delete the string for a given key.
        /// </summary>
        /// <returns>True if the deletion was successful, false otherwise.</returns>
        /// <param name="key">The key for the key-value store.</param>
        bool Delete(string key);
    }
}
