// <copyright file="Exception.extensions.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using Float.Core.Net;

namespace Float.Core.Extensions
{
    /// <summary>
    /// Useful extensions on the <see cref="Exception"/> class.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Determines if the given extension is an error related to network connectivity issues.
        /// </summary>
        /// <param name="e">The exception to check for being related to offline status.</param>
        /// <returns><c>true</c> if the exception is related to being offline, <c>false</c> otherwise.</returns>
        public static bool IsOfflineException(this Exception e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // check for our own offline exception type (not useful in Float Core directly, but useful for consumers of this project)
            if (e is HttpConnectionException)
            {
                return true;
            }

            // on Android, we may get a native Java exception related to offline status
            var javaIOExceptionType = Type.GetType("Java.IO.IOException, Mono.Android");

            if (javaIOExceptionType != null && javaIOExceptionType.GetTypeInfo().IsAssignableFrom(e.GetType().GetTypeInfo()))
            {
                return true;
            }

            // we sometimes get a webexception status of NameResolutionFailure that we don't have access to, but its enum value is one
            if (e.InnerException is WebException webException && ((int)webException.Status == 1 || webException.Status == WebExceptionStatus.ConnectFailure))
            {
                return true;
            }

            // on iOS, we may get an NSException with a status code of -1009, indicating the device is offline
            if (e.InnerException is Exception innerException)
            {
                var innerExceptionType = innerException.GetType();

                if (innerExceptionType.Name == "NSErrorException")
                {
                    try
                    {
                        var code = innerExceptionType.GetProperty("Code")?.GetValue(innerException);

                        if (code != null)
                        {
                            switch (Convert.ToInt64(code, new NumberFormatInfo()))
                            {
                                // CannotFindHost
                                // CannotConnectToHost
                                // NetworkConnectionLost
                                // CannotLoadFromNetwork
                                case long n when n >= -1006 && n <= -1003:
                                    return true;
                                case -1009: // DNSLookupFailed
                                    return true;
                                case -1020: // DataNotAllowed
                                    return true;
                                case -2000: // CannotLoadFromNetwork
                                    return true;
                            }
                        }
                    }
                    catch (InvalidCastException)
                    {
                        // if GetValue fails, we may end up here
                    }
                }
            }

            return false;
        }
    }
}
