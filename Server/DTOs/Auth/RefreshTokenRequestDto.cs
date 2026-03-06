using System.ComponentModel.DataAnnotations;

namespace brevet_tracker.Server.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        /// <summary>
        /// Gets or sets the current access token.
        /// </summary>
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the refresh token used to obtain a new access token.
        /// </summary>
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
