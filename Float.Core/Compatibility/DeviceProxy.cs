using System;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Float.Core.Compatibility
{
    /// <summary>
    /// Uses reflection to access Forms-only methods on the device type, like BeginInvokeOnMainThread.
    /// </summary>
    internal static class DeviceProxy
    {
        /// <summary>
        /// Attempts to call the Xamarin.Forms main thread switching method.
        /// When Forms is not available, this calls the provided action directly.
        /// </summary>
        /// <param name="action">The action to invoke on the main thread.</param>
        internal static void BeginInvokeOnMainThread(Action action)
        {
            try
            {
                var method = typeof(Device).GetMethod("BeginInvokeOnMainThread", BindingFlags.Static | BindingFlags.Public);
                method?.Invoke(null, new[] { action });
            }
            catch (Exception e) when (e is TargetInvocationException || e is InvalidOperationException)
            {
                action.Invoke();
            }
        }

        internal static async Task InvokeOnMainThreadAsync(Action action)
        {
            try
            {
                var method = typeof(Device).GetMethod("InvokeOnMainThreadAsync", BindingFlags.Static | BindingFlags.Public);
                await method?.InvokeAsync(null, new[] { action });
            }
            catch (Exception e) when (e is TargetInvocationException || e is InvalidOperationException)
            {
                var result = action.BeginInvoke(null, null);
                action.EndInvoke(result);
            }
        }

        internal static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
        {
            var task = (Task)@this.Invoke(obj, parameters);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }
    }
}
