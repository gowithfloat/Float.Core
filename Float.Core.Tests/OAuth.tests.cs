// <copyright file="OAuth.tests.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Linq;
using System.Threading.Tasks;
using Float.Core.Net;
using Xunit;

namespace Float.Core.Tests
{
    public class OAuthTests
    {
        const string ClientId = "***REMOVED***";
        const string ClientSecret = "***REMOVED***";
        const string Username = "***REMOVED***";
        const string Password = "***REMOVED***";
        readonly Uri Url = new ("***REMOVED***");

        [Fact(Skip = "tk")]
        public async Task TestOAuthAsync()
        {
            var auth = new OAuth2Strategy(Url, ClientId, ClientSecret, new NonPersistentSecureStore(), grantType: OAuth2GrantType.ClientCredentials);
            Assert.True(await auth.Login());
        }

        [Fact(Skip = "tk")]
        public async Task TestRequestWithAuthAsync()
        {
            var auth = new OAuth2Strategy(Url, ClientId, ClientSecret, new NonPersistentSecureStore(), grantType: OAuth2GrantType.Password);
            _ = await auth.Login(Username, Password);

            var client = new RequestClient(Url, auth);
            var response = await client.Get("/api/browse");
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotEmpty(response.Content);
        }

        [Fact(Skip = "tk")]
        public async Task TestBatchRequestsWithAuthAsync()
        {
            var auth = new OAuth2Strategy(Url, ClientId, ClientSecret, new NonPersistentSecureStore(), grantType: OAuth2GrantType.Password);
            _ = await auth.Login(Username, Password);
            var client = new RequestClient(Url, auth);

            // verify the tokens are valid
            var beforeTokens = await auth.GetTokens().ConfigureAwait(false);
            Assert.False(beforeTokens.ShouldRefresh);

            // replace access token with invalid string
            await auth.StoreTokens(access: "bad access token");

            // queue up 10 tasks that will each attempt to get a new access token
            var tcs = new TaskCompletionSource<object>();
            var tasks = Enumerable.Repeat(DoStuff(tcs.Task, client, auth), 10);

            // move the task completion source to done, starting all 10 tasks at once
            Assert.True(tcs.TrySetResult(default));

            // wait for all tasks to complete
            var responses = await Task.WhenAll(tasks);

            // just in case, verify the number of responses
            Assert.True(responses.Count() == 10);

            // verify we got the same token for each
            Assert.Single(responses.Distinct());
        }

        [Fact(Skip = "tk")]
        public async Task TestLogout()
        {
            var auth = new OAuth2Strategy(Url, ClientId, ClientSecret, new NonPersistentSecureStore(), grantType: OAuth2GrantType.Password);
            _ = await auth.Login(Username, Password);
            Assert.True(auth.IsAuthenticated == true);

            _ = await auth.Logout();
            Assert.True(auth.IsAuthenticated == false);
        }

        async Task<string> DoStuff(Task start, RequestClient client, OAuth2Strategy strat)
        {
            await start;

            try
            {
                await client.Get("/api/browse");
            }
            catch (HttpRequestException)
            {
            }

            return await strat.RawRefreshToken();
        }
    }
}
