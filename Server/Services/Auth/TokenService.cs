using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using brevet_tracker.Server.Models.Auth;
using brevet_tracker.Server.Models.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace brevet_tracker.Server.Services.Auth
{
    /// <summary>
    /// Provides JWT access and refresh token operations for authentication flows.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TokenService> _logger;
        private readonly JwtSettings _jwtSettings;

        public TokenService(
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            ILogger<TokenService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _jwtSettings = _configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                ?? throw new InvalidOperationException($"{JwtSettings.SectionName} configuration section is missing.");

            if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey) || _jwtSettings.SecretKey.Length < 32)
            {
                throw new InvalidOperationException("JwtSettings:SecretKey must be configured with at least 32 characters.");
            }

            if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer) || string.IsNullOrWhiteSpace(_jwtSettings.Audience))
            {
                throw new InvalidOperationException("JwtSettings:Issuer and JwtSettings:Audience must be configured.");
            }
        }

        /// <inheritdoc />
        public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    claims.Add(new Claim(ClaimTypes.Email, user.Email));
                    claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
                }

                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var token = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    notBefore: DateTime.UtcNow,
                    expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
                    signingCredentials: GetSigningCredentials());

                var serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation("Access token generated for user {UserId}.", user.Id);
                return serializedToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate access token for user {UserId}.", user.Id);
                throw;
            }
        }

        /// <inheritdoc />
        public Task<string> GenerateRefreshTokenAsync()
        {
            try
            {
                // 48 bytes -> 64 base64 characters.
                var randomBytes = new byte[48];
                RandomNumberGenerator.Fill(randomBytes);
                var refreshToken = Convert.ToBase64String(randomBytes)
                    .Replace('+', '-')
                    .Replace('/', '_');

                return Task.FromResult(refreshToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate refresh token.");
                throw;
            }
        }

        /// <inheritdoc />
        public Task<ClaimsPrincipal> GetPrincipalFromExpiredTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));
            }

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = false,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidAudience = _jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
                {
                    throw new SecurityTokenException("Invalid token.");
                }

                return Task.FromResult(principal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract principal from expired token.");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return false;
            }

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("Refresh token validation failed. User {UserId} was not found.", userId);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(user.RefreshToken))
                {
                    _logger.LogWarning("No stored refresh token found for user {UserId}.", userId);
                    return false;
                }

                if (!user.RefreshTokenExpiryTime.HasValue || user.RefreshTokenExpiryTime.Value <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Refresh token has expired for user {UserId}.", userId);
                    return false;
                }

                var providedBytes = Encoding.UTF8.GetBytes(refreshToken);
                var storedBytes = Encoding.UTF8.GetBytes(user.RefreshToken);

                return CryptographicOperations.FixedTimeEquals(providedBytes, storedBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate refresh token for user {UserId}.", userId);
                return false;
            }
        }

        private SigningCredentials GetSigningCredentials()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
    }
}
