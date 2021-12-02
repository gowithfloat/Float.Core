// <copyright file="UrlExtensions.tests.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System.Text.RegularExpressions;
using Float.Core.Extensions;
using Xunit;

namespace Float.Core.Tests
{
    /// <summary>
    /// URL extensions test.
    /// </summary>
    public class UrlExtensionsTest
    {
        /// <summary>
        /// Tests the regex extension.
        /// </summary>
        [Fact]
        public void TestRegexExtension()
        {
            var urlToMatch = "https://www.gowithsparklearn.com/auth";
            var regex = new Regex(urlToMatch.PatternForMatchingEitherHost());

            Assert.Matches(regex, urlToMatch);

            urlToMatch = "https://gowithsparklearn.com/auth";
            Assert.Matches(regex, urlToMatch);

            urlToMatch = "https://auth.gowithsparklearn.com/auth";
            Assert.DoesNotMatch(regex, urlToMatch);

            urlToMatch = "https://auth.gowithsparklearn.com";
            Assert.DoesNotMatch(regex, urlToMatch);

            urlToMatch = "https://sparkler-pilota-dev.gowithfloat.net/saml_login?destination=/oauth/authorize%3Fclient_id%3D21f9f104-2b73-4b37-a38f-d32fd421a159%26redirect_uri%3Dhttps%253A//gowithsparklearn.com/auth%26response_type%3Dcode";
            Assert.DoesNotMatch(regex, urlToMatch);

            urlToMatch = "https://gowithsparklearn.com/auth/";
            Assert.Matches(regex, urlToMatch);
        }

        [Fact]
        public void TestRegexExtensionEvenMore()
        {
            var urlToMatch = "https://www.gowithsparklearn.com/auth/";
            var regex = new Regex(urlToMatch.PatternForMatchingEitherHost());
            Assert.Matches(regex, urlToMatch);

            urlToMatch = "https://gowithsparklearn.com/auth";
            Assert.Matches(regex, urlToMatch);
        }
    }
}
