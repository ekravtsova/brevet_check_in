using System;
using System.ComponentModel.DataAnnotations;

namespace brevet_tracker.Server.DTOs.Auth
{
    public class TokenResponseDto
    {
        /// <summary>
        /// Gets or sets the JWT access token.
        /// </summary>
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the refresh token value.
        /// </summary>
        [Required]
        public string RefreshToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the UTC date and time when the access token expires.
        /// </summary>
        public DateTime ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets the token type. Defaults to "Bearer".
        /// </summary>
        [Required]
        public string TokenType { get; set; } = "Bearer";
    }
}
