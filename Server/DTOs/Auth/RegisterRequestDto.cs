using System.ComponentModel.DataAnnotations;

namespace brevet_tracker.Server.DTOs.Auth
{
    public class RegisterRequestDto
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

        /// <summary>
        /// Gets or sets the user's optional display name.
        /// </summary>
        [StringLength(100)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the optional role requested during registration.
        /// Defaults to Participant when omitted.
        /// </summary>
        [RegularExpression("^(Admin|Participant)$", ErrorMessage = "Role must be either 'Admin' or 'Participant'.")]
        public string? Role { get; set; }
    }
}
