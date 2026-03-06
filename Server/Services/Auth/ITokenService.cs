using System.Threading.Tasks;
using System.Security.Claims;
using brevet_tracker.Server.Models.Auth;

namespace brevet_tracker.Server.Services.Auth
{
    /// <summary>
    /// Defines operations for creating and validating JWT access and refresh tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a signed JWT access token for the specified user.
        /// </summary>
        /// <param name="user">The authenticated application user.</param>
        /// <returns>The generated access token string.</returns>
        Task<string> GenerateAccessTokenAsync(ApplicationUser user);

        /// <summary>
        /// Generates a cryptographically secure refresh token.
        /// </summary>
        /// <returns>The generated refresh token string.</returns>
        Task<string> GenerateRefreshTokenAsync();

        /// <summary>
        /// Extracts and returns claims principal data from an expired access token.
        /// </summary>
        /// <param name="token">The expired JWT access token.</param>
        /// <returns>The claims principal extracted from the token.</returns>
        Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token);

        /// <summary>
        /// Validates that the provided refresh token is valid for the specified user.
        /// </summary>
        /// <param name="userId">The application user identifier.</param>
        /// <param name="refreshToken">The refresh token to validate.</param>
        /// <returns>True if the refresh token is valid; otherwise, false.</returns>
        Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);
    }
}
