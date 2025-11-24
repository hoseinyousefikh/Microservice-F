using Identity_Service.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity_Service.Infrastructure.Persistence.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.HasKey(prt => prt.Id);

            builder.Property(prt => prt.Token)
                .IsRequired()
                .HasMaxLength(512);

            builder.Property(prt => prt.ExpiresAt)
                .IsRequired();

            builder.Property(prt => prt.CreatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(prt => prt.User)
                .WithMany(u => u.PasswordResetTokens)
                .HasForeignKey(prt => prt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index
            builder.HasIndex(prt => prt.Token)
                .IsUnique();
        }
    }
}
