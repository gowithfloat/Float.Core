// <copyright file="Dictionary.extensions.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System.Collections.Generic;

namespace Float.Core.Extensions
{
    /// <summary>
    /// Dictionary extensions.
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// Merge the specified dictionary and dictionaries. Duplicate keys are skipped, not overwritten.
        /// </summary>
        /// <returns>The merged dictionary.</returns>
        /// <param name="thisDictionary">This dictionary.</param>
        /// <param name="dictionaries">Enumerable of dictionaries.</param>
        /// <typeparam name="TKey">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> thisDictionary, IEnumerable<Dictionary<TKey, TValue>> dictionaries)
        {
            if (dictionaries == null)
            {
                return thisDictionary;
            }

            foreach (var dictionary in dictionaries)
            {
                thisDictionary?.Merge(dictionary);
            }

            return thisDictionary;
        }

        /// <summary>
        /// Merge this dictionary and the given dictionary. If the value already exists in the dictionary, it is skipped.
        /// </summary>
        /// <returns>The merged dictionary.</returns>
        /// <param name="thisDictionary">This dictionary.</param>
        /// <param name="dictionary">The Dictionary to merge.</param>
        /// <typeparam name="TKey">The 1st type parameter.</typeparam>
        /// <typeparam name="TValue">The 2nd type parameter.</typeparam>
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> thisDictionary, Dictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null)
            {
                return thisDictionary;
            }

            if (thisDictionary == null)
            {
                return dictionary;
            }

            foreach (var kvp in dictionary)
            {
                if (!thisDictionary.ContainsKey(kvp.Key))
                {
                    thisDictionary.Add(kvp.Key, kvp.Value);
                }
            }

            return thisDictionary;
        }
    }
}
