using System.ComponentModel.DataAnnotations;

namespace brevet_tracker.Server.Models.Settings
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        /// <summary>
        /// Gets or sets the secret key used to sign JWTs.
        /// </summary>
        [Required]
        [MinLength(32)]
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token issuer.
        /// </summary>
        [Required]
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the token audience.
        /// </summary>
        [Required]
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets access token expiration time in minutes.
        /// </summary>
        [Range(1, 1440)]
        public int AccessTokenExpirationMinutes { get; set; } = 15;

        /// <summary>
        /// Gets or sets refresh token expiration time in days.
        /// </summary>
        [Range(1, 365)]
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
