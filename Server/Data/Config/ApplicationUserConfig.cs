using brevet_tracker.Server.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace brevet_tracker.Server.Data.Config
{
    /// <summary>
    /// Configures ApplicationUser-specific persistence settings.
    /// </summary>
    public class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.RefreshToken)
                .HasMaxLength(512)
                .IsRequired(false);

            builder.Property(u => u.RefreshTokenExpiryTime)
                .IsRequired(false);

            builder.Property(u => u.LastTokenRequestTime)
                .IsRequired(false);
        }
    }
}
