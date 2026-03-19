using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Piloopinas.Domain.Entities;

namespace Piloopinas.Infrastructure.Data.Configurations;

public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Name).HasMaxLength(100).IsRequired();
        builder.Property(b => b.Description).HasMaxLength(500);
        builder.Property(b => b.IconUrl).HasMaxLength(500);
        builder.Property(b => b.Category).HasMaxLength(50).IsRequired();
        builder.Property(b => b.CriteriaType).HasMaxLength(50).IsRequired();
        
        builder.HasIndex(b => new { b.Category, b.IsActive });
    }
}

public class UserBadgeConfiguration : IEntityTypeConfiguration<UserBadge>
{
    public void Configure(EntityTypeBuilder<UserBadge> builder)
    {
        builder.HasKey(ub => ub.Id);
        
        builder.Property(ub => ub.Notes).HasMaxLength(500);
        
        builder.HasIndex(ub => new { ub.UserId, ub.BadgeId }).IsUnique();
        
        builder.HasOne(ub => ub.User)
            .WithMany(u => u.UserBadges)
            .HasForeignKey(ub => ub.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(ub => ub.Badge)
            .WithMany(b => b.UserBadges)
            .HasForeignKey(ub => ub.BadgeId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
        
        builder.Property(rt => rt.TokenHash).HasMaxLength(256).IsRequired();
        builder.Property(rt => rt.CreatedByIp).HasMaxLength(50);
        builder.Property(rt => rt.RevokedByIp).HasMaxLength(50);
        builder.Property(rt => rt.ReplacedByTokenHash).HasMaxLength(256);
        
        builder.HasIndex(rt => rt.TokenHash);
        builder.HasIndex(rt => new { rt.UserId, rt.ExpiresAt });
        
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
