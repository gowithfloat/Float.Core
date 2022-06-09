using System;
using System.Threading.Tasks;
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
        public async void Serializable()
        {
            var tokens = new OAuthTokens("access token", "refresh token", 10*60+1);
            Assert.False(tokens.ShouldRefresh);
            var serializedData = JsonConvert.SerializeObject(tokens);

            // OAuthTokens are configured to proactively refresh 10 minutes before they expire.
            // Since we've set our token time to 10 minutes and 1 second, after waiting just over
            // one second here, the token should now think it's time to be refreshed.
            await Task.Delay(1100);

            var deserializedTokens = JsonConvert.DeserializeObject<OAuthTokens>(serializedData);

            Assert.True(tokens.ShouldRefresh, "The test ran too fast");
            Assert.True(deserializedTokens.ShouldRefresh, "The deserialized token has forgotten it should be refreshed");
        }
    }
}

