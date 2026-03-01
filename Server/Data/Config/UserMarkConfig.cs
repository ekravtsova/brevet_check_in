using brevet_tracker.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace brevet_tracker.Server.Data.Config
{
    public class UserMarkConfig : IEntityTypeConfiguration<UserMark>
    {
        public void Configure(EntityTypeBuilder<UserMark> builder)
        {
            builder.ToTable("user_marks");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId).IsRequired();
            builder.Property(x => x.CheckpointId).IsRequired();
            builder.Property(x => x.PassedAt).IsRequired();
            builder.Property(x => x.IsSynced).HasDefaultValue(false).IsRequired();
            builder.Property(x => x.DeviceInfo).HasMaxLength(200).IsRequired(false);
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();

            builder
                .HasOne(x => x.User)
                .WithMany(x => x.Marks)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(x => x.Checkpoint)
                .WithMany(x => x.UserMarks)
                .HasForeignKey(x => x.CheckpointId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasIndex(x => new { x.UserId, x.CheckpointId })
                .IsUnique();
        }
    }
}
