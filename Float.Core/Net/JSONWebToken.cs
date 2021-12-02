// <copyright file="JSONWebToken.cs" company="Float">
// Copyright (c) 2021 Float, All rights reserved.
// Shared under an MIT license. See license.md for details.
// </copyright>

using System;
using System.Collections.Generic;
using Float.Core.Exceptions;
using Float.Core.Extensions;
using Newtonsoft.Json.Linq;

namespace Float.Core.Net
{
    /// <summary>
    /// Storage for JSON web token data per https://tools.ietf.org/html/rfc7519#section-4.1.
    /// </summary>
    public struct JSONWebToken : IEquatable<JSONWebToken>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JSONWebToken"/> struct.
        /// </summary>
        /// <param name="token">The encoded token data to convert to decoded data.</param>
        public JSONWebToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidStringArgumentException(nameof(token));
            }

            var split = token.Split('.');

            if (split.Length < 3)
            {
                throw new ArgumentException("Given token did not have two periods.");
            }

            var parsedHeader = JObject.Parse(split[0].FromUrlEncodedBase64());
            var parsedPayload = JObject.Parse(split[1].FromUrlEncodedBase64());

            Type = parsedHeader.Value<string>("typ");
            Algorithm = parsedHeader.Value<string>("alg");
            Audience = parsedPayload.Value<string>("aud");
            JwtId = parsedPayload.Value<string>("jti");
            IssuedAt = FromSecondsSinceEpoch(parsedPayload.Value<long>("iat"));
            NotBefore = FromSecondsSinceEpoch(parsedPayload.Value<long>("nbf"));
            ExpirationTime = FromSecondsSinceEpoch(parsedPayload.Value<long>("exp"));
            Subject = parsedPayload.Value<string>("sub");
        }

        /// <summary>
        /// Gets the "typ" (type) Header Parameter defined by JWS and JWE is used by JWT applications to declare the media type of this complete JWT.
        /// </summary>
        /// <value>A string represening the type of the token.</value>
        public string Type { get; }

        /// <summary>
        /// Gets the algorithm used to sign this token.
        /// </summary>
        /// <value>A string represening the algorithm used to sign this token.</value>
        public string Algorithm { get; }

        /// <summary>
        /// Gets the "aud" (audience) claim identifies the recipients that the JWT is intended for.
        /// </summary>
        /// <value>A string identifying the intended recipients.</value>
        public string Audience { get; }

        /// <summary>
        /// Gets the "jti" (JWT ID) claim provides a unique identifier for the JWT.
        /// </summary>
        /// <value>A string representing the claim ID.</value>
        public string JwtId { get; }

        /// <summary>
        /// Gets the "iat" (issued at) claim identifies the time at which the JWT was issued.
        /// </summary>
        /// <value>The timestamp when the token was issued.</value>
        public DateTime IssuedAt { get; }

        /// <summary>
        /// Gets the "nbf" (not before) claim identifies the time before which the JWT MUST NOT be accepted for processing.
        /// </summary>
        /// <value>The timestamp representing when the token becomes valid.</value>
        public DateTime NotBefore { get; }

        /// <summary>
        /// Gets the "exp" (expiration time) claim identifies the expiration time on or after which the JWT MUST NOT be accepted for processing.
        /// </summary>
        /// <value>The timestamp representing when the token expires.</value>
        public DateTime ExpirationTime { get; }

        /// <summary>
        /// Gets the "sub" (subject) claim identifies the principal that is the subject of the JWT.
        /// </summary>
        /// <value>A string representing the subject of the token.</value>
        public string Subject { get; }

        /// <summary>
        /// Determines whether two object instances are inequal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if the objects are considered inequal, <c>false</c> otherwise.</returns>
        public static bool operator ==(JSONWebToken left, JSONWebToken right) => left.Equals(right);

        /// <summary>
        /// Determines whether two object instances are inequal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns><c>true</c> if the objects are considered inequal, <c>false</c> otherwise.</returns>
        public static bool operator !=(JSONWebToken left, JSONWebToken right) => !(left == right);

        /// <inheritdoc />
        public override string ToString()
        {
            return $"[JWT Type: {Type}, Algorithm: {Algorithm}, Audience: {Audience}, JWTID: {JwtId}, IssuedAt: {IssuedAt}, NotBefore: {NotBefore}, ExpirationTime: {ExpirationTime}, Subject: {Subject}]";
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is JSONWebToken token && Equals(token);
        }

        /// <inheritdoc />
        public bool Equals(JSONWebToken other)
        {
            return other.GetHashCode() == GetHashCode();
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 54149947 + EqualityComparer<string>.Default.GetHashCode(JwtId);
        }

        static DateTime FromSecondsSinceEpoch(long seconds)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
        }
    }
}
