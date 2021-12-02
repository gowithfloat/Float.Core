// <copyright file="AnalyticsMessageType.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

namespace Float.Core.Analytics
{
    /// <summary>
    /// Messages posted to the MessagingCenter for analytics.
    /// </summary>
    static class AnalyticsMessageType
    {
        /// <summary>
        /// Gets message name for a page view.
        /// </summary>
        /// <value>Message name for a page view.</value>
        internal static string PageView => "PageView";
    }
}
