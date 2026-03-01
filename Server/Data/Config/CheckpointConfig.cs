using brevet_tracker.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace brevet_tracker.Server.Data.Config
{
    public class CheckpointConfig : IEntityTypeConfiguration<Checkpoint>
    {
        public void Configure(EntityTypeBuilder<Checkpoint> builder)
        {
            builder.ToTable("checkpoints");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.BrevetId).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Latitude).IsRequired();
            builder.Property(x => x.Longitude).IsRequired();
            builder.Property(x => x.Order).IsRequired();
            builder.Property(x => x.CreatedAt).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();

            builder
                .HasOne(x => x.Brevet)
                .WithMany(x => x.Checkpoints)
                .HasForeignKey(x => x.BrevetId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
