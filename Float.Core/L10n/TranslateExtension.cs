using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Float.Core.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Float.Core.L10n
{
    /// <summary>
    /// The markup extension allowing XAML to contain localization.
    /// </summary>
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        static readonly List<ResourceManager> RegisteredResourceManagers = new ()
        {
            FloatStrings.ResourceManager,
        };

        /// <summary>
        /// Gets the CultureInfo.
        /// </summary>
        readonly CultureInfo ci = CultureInfo.CurrentUICulture;

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslateExtension" /> class.
        /// </summary>
        public TranslateExtension()
        {
        }

        /// <summary>
        /// Gets or sets the Text.
        /// </summary>
        /// <value>The string which should be translated.</value>
        public string Text { get; set; }

        /// <summary>
        /// Registers the resource manager.
        /// </summary>
        /// <param name="resmgr">The resource manager to register.</param>
        public static void RegisterResourceManager(ResourceManager resmgr)
        {
            RegisteredResourceManagers.Insert(0, resmgr);
        }

        /// <summary>
        /// Unregisters the resource manager.
        /// </summary>
        /// <param name="resmgr">The resource manager to unregister.</param>
        public static void UnregisterResourceManager(ResourceManager resmgr)
        {
            RegisteredResourceManagers.Remove(resmgr);
        }

        /// <summary>
        /// Registers the float core resource manager.
        /// </summary>
        [Obsolete("RegisterFloatCoreResourceManager is deprecated; these strings are automatically registered now.")]
        public static void RegisterFloatCoreResourceManager()
        {
        }

        /// <summary>
        /// Unregisters the float core resource manager.
        /// </summary>
        [Obsolete("UnregisterFloatCoreResourceManager is deprecated; there is no replacement.")]
        public static void UnregisterFloatCoreResourceManager()
        {
        }

        /// <summary>
        /// Provides the translated value for the provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider for the value.</param>
        /// <returns>The translated value.</returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
            {
                return string.Empty;
            }

            var translation = FindLocalization(Text, ci);

            if (translation == null)
            {
#if DEBUG
                throw new KeyNotFoundException(Text);
#else
                translation = Text;
#endif
            }

            return translation;
        }

        internal static string FindLocalization(string key, CultureInfo ci)
        {
            foreach (var resmgr in RegisteredResourceManagers)
            {
                var result = resmgr.GetString(key, ci);

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
