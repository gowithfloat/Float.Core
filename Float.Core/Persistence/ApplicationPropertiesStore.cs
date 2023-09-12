using Float.Core.Exceptions;
using Float.Core.Extensions;
#if NETSTANDARD
using Xamarin.Forms;
#else
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
#endif

namespace Float.Core.Persistence
{
    /// <summary>
    /// A non-secure store that stores information in Xamarin's application properties dictionary.
    /// </summary>
    public class ApplicationPropertiesStore : IKeyValueStore
    {
        /// <summary>
        /// Delete the specified key from the application properties dictionary.
        /// </summary>
        /// <param name="key">The key of the value to remove.</param>
        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidStringArgumentException(nameof(key));
            }
#if NETSTANDARD
            // this is null when the app first starts up
            if (Application.Current == null)
            {
                return;
            }

            // trying to get a non-existent key doesn't just return null, it causes an exception
            if (!Application.Current.Properties.ContainsKey(key))
            {
                return;
            }

            Application.Current.Properties.Remove(key);
            Save();
#else
            if (!Preferences.Default.ContainsKey(key))
            {
                return;
            }

            Preferences.Default.Remove(key);
#endif
        }

        /// <summary>
        /// Get the specified key from the application properties dictionary.
        /// </summary>
        /// <returns>The value that was found in the dictionary, or a defalt value if non was found.</returns>
        /// <param name="key">The key of the value to find.</param>
        /// <typeparam name="T">The expected type of the object to retrieve.</typeparam>
        public T Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidStringArgumentException(nameof(key));
            }

            // this is null when the app first starts up
            if (Application.Current == null)
            {
                return default;
            }
#if NETSTANDARD

            // trying to get a non-existent key doesn't just return null, it causes an exception
            if (!Application.Current.Properties.ContainsKey(key))
            {
                return default;
            }

            var obj = Application.Current.Properties[key];
#else
            if (!Preferences.Default.ContainsKey(key))
            {
                return default;
            }

            var obj = Preferences.Default.Get<T>(key, default);
#endif

            if (obj is T t)
            {
                return t;
            }

            return default;
        }

        /// <summary>
        /// Put the specified key and value in the application properties dictionary.
        /// </summary>
        /// <param name="key">The key of the value to store.</param>
        /// <param name="value">The value to store in the application properties.</param>
        /// <typeparam name="T">The type of the object to store.</typeparam>
        public void Put<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidStringArgumentException(nameof(key));
            }

            // this is null when the app first starts up
            if (Application.Current == null)
            {
                return;
            }
#if NETSTANDARD

            Application.Current.Properties[key] = value;
            Save();
#else
            Preferences.Default.Set(key, value);
#endif
        }

#if NETSTANDARD
        static void Save()
        {
            Application.Current.SavePropertiesAsync().OnFailure(task =>
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Failed to save properties: {task?.Exception?.Message}");
#endif
            });
        }
#endif
    }
}
