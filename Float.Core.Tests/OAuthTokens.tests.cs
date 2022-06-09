using System;
using Float.Core.Net;
using Newtonsoft.Json;
using Xunit;

namespace Float.Core.Tests
{
    public class OAuthTokensTests
    {
        [Fact]
        public void ComputesExpirationDate()
        {
            var now = DateTime.Now;
            var duration = 10;
            var tokens = new OAuthTokens("access token", "refresh token", duration, now);

            Assert.Equal(now.AddSeconds(duration), tokens.Expires);
        }

        [Fact]
        public void ShouldRefreshWithinTenMinutes()
        {
            var fifteenMinuteToken = new OAuthTokens("access token", "refresh token", 15 * 60);
            var tenMinuteToken = new OAuthTokens("access token", "refresh token", 10 * 60);
            var zeroMinuteToken = new OAuthTokens("access token", "refresh token", 0);
            var expiredToken = new OAuthTokens("access token", "refresh token", -10);

            Assert.False(fifteenMinuteToken.ShouldRefresh);
            Assert.True(tenMinuteToken.ShouldRefresh);
            Assert.True(zeroMinuteToken.ShouldRefresh);
            Assert.True(expiredToken.ShouldRefresh);
        }

        [Fact]
        public void SerializingRetainsExpirationDate()
        {
            var tokens = new OAuthTokens("access token", "refresh token", 1000);
            var serializedData = JsonConvert.SerializeObject(tokens);
            var deserializedTokens = JsonConvert.DeserializeObject<OAuthTokens>(serializedData);

            Assert.Equal(tokens.Expires, deserializedTokens.Expires);
        }
    }
}

