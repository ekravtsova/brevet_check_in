using System;
using System.Linq;
using System.Threading.Tasks;
using brevet_tracker.Server.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace brevet_tracker.Server.Services.Auth
{
    /// <summary>
    /// Handles persistence of refresh token values on the application user entity.
    /// </summary>
    public class UserRefreshTokenService : IUserRefreshTokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserRefreshTokenService> _logger;

        public UserRefreshTokenService(
            UserManager<ApplicationUser> userManager,
            ILogger<UserRefreshTokenService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task UpdateRefreshTokenAsync(string userId, string refreshToken, DateTime expiry)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User id is required.", nameof(userId));
            }

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentException("Refresh token is required.", nameof(refreshToken));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"User '{userId}' was not found.");
            }

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = expiry.ToUniversalTime();
            user.LastTokenRequestTime = DateTime.UtcNow;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to update refresh token for user {UserId}. Errors: {Errors}", userId, errors);
                throw new InvalidOperationException("Failed to update refresh token.");
            }
        }
    }
}
