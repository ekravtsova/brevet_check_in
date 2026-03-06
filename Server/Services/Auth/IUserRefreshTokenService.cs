using System;
using System.Threading.Tasks;

namespace brevet_tracker.Server.Services.Auth
{
    /// <summary>
    /// Provides operations for persisting refresh token data for users.
    /// </summary>
    public interface IUserRefreshTokenService
    {
        /// <summary>
        /// Updates a user's refresh token and its expiry time.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="refreshToken">The new refresh token value.</param>
        /// <param name="expiry">The refresh token expiration date and time (UTC).</param>
        Task UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime expiry);
    }
}
