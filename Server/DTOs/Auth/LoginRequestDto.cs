using System.ComponentModel.DataAnnotations;

namespace brevet_tracker.Server.DTOs.Auth
{
    public class LoginRequestDto
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
