using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using brevet_tracker.Server.DTOs.Auth;
using brevet_tracker.Server.Models.Auth;
using brevet_tracker.Server.Models.Settings;
using brevet_tracker.Server.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace brevet_tracker.Server.Controllers
{
    /// <summary>
    /// Provides authentication endpoints for login, registration, token refresh, and logout.
    /// </summary>
    [Route("api/[controller]")]
    public class AuthController : AuthorizedApiControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUserRefreshTokenService _userRefreshTokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly JwtSettings _jwtSettings;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IUserRefreshTokenService userRefreshTokenService,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userRefreshTokenService = userRefreshTokenService ?? throw new ArgumentNullException(nameof(userRefreshTokenService));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                ?? throw new InvalidOperationException("JwtSettings configuration is missing.");
        }

        /// <summary>
        /// Authenticates a user and returns JWT access and refresh tokens.
        /// </summary>
        /// <param name="request">Login request payload.</param>
        /// <returns>A token response when authentication succeeds.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!passwordCheck.Succeeded)
            {
                return Unauthorized("Invalid credentials.");
            }

            var tokenResponse = await CreateAndStoreTokensAsync(user);
            return Ok(tokenResponse);
        }

        /// <summary>
        /// Registers a new user account and returns JWT access and refresh tokens.
        /// </summary>
        /// <param name="request">Registration request payload.</param>
        /// <returns>A token response when registration succeeds.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest("A user with this email already exists.");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                DisplayName = request.DisplayName,
                RegisteredAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, request.Password);
            if (!createResult.Succeeded)
            {
                var errors = createResult.Errors.Select(e => e.Description).ToArray();
                return BadRequest(new { Errors = errors });
            }

            var tokenResponse = await CreateAndStoreTokensAsync(user);
            return Ok(tokenResponse);
        }

        /// <summary>
        /// Validates an expired access token and refresh token pair, then issues new tokens.
        /// </summary>
        /// <param name="request">Refresh token request payload.</param>
        /// <returns>A new token response when refresh succeeds.</returns>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ClaimsPrincipal principal;
            try
            {
                principal = await _tokenService.GetPrincipalFromExpiredTokenAsync(request.AccessToken);
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid access token.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while reading expired access token.");
                return Unauthorized("Invalid access token.");
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue(JwtRegisteredClaimNames.Sub);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized("Invalid token claims.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var isRefreshTokenValid = await _tokenService.ValidateRefreshTokenAsync(userId, request.RefreshToken);
            if (!isRefreshTokenValid)
            {
                return Unauthorized("Invalid refresh token.");
            }

            var tokenResponse = await CreateAndStoreTokensAsync(user);
            return Ok(tokenResponse);
        }

        /// <summary>
        /// Logs out the current user and invalidates the stored refresh token.
        /// </summary>
        /// <returns>No content when logout succeeds.</returns>
        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.LastTokenRequestTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync();

            return Ok();
        }

        private async Task<TokenResponseDto> CreateAndStoreTokensAsync(ApplicationUser user)
        {
            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _tokenService.GenerateRefreshTokenAsync();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            await _userRefreshTokenService.UpdateRefreshTokenAsync(user.Id, refreshToken, refreshTokenExpiry);

            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = jwtToken.ValidTo,
                TokenType = "Bearer"
            };
        }
    }
}
