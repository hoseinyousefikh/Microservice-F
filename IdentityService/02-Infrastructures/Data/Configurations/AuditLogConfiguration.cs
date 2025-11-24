using IdentityService._01_Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService._02_Infrastructures.Data.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");

            builder.HasKey(al => al.Id);

            builder.Property(al => al.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.Entity)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(al => al.EntityId)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(al => al.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(al => al.NewValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(al => al.IpAddress)
                .HasMaxLength(45);

            builder.Property(al => al.UserAgent)
                .HasMaxLength(500);

            builder.Property(al => al.Timestamp)
                .IsRequired();

            builder.HasIndex(al => al.Timestamp);
            builder.HasIndex(al => al.UserId);
            builder.HasIndex(al => al.Action);
            builder.HasIndex(al => al.Entity);
        }
    }
}
