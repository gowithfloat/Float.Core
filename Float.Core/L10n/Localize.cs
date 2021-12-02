// <copyright file="Localize.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System.Globalization;

namespace Float.Core.L10n
{
    /// <summary>
    /// Localize a value.
    /// </summary>
    public static class Localize
    {
        /// <summary>
        /// Localize a string in the specified culture (defaults to current culture).
        /// </summary>
        /// <returns>The localized string or the key if no localized value was found.</returns>
        /// <param name="key">The key.</param>
        /// <param name="ci">The target culture (defaults to current UI culture).</param>
        /// <remarks>
        /// Currently this class relies on the registered resource managers in TranslateExtension.
        /// Eventually, Float.Core should be able to search assemblies automatically to find
        /// resource managers and the dependency can flip so <see cref="TranslateExtension" />
        /// relies on this class.
        /// </remarks>
        public static string String(string key, CultureInfo ci = null)
        {
            var culture = ci ?? CultureInfo.CurrentUICulture;
            var localizedValue = TranslateExtension.FindLocalization(key, culture);
            return localizedValue ?? key;
        }
    }
}
