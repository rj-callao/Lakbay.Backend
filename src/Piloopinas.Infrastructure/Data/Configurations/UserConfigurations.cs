using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Lakbay.Domain.Entities;

namespace Lakbay.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        builder.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasMaxLength(100).IsRequired();
        builder.Property(u => u.ProfilePhotoUrl).HasMaxLength(500);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.Address).HasMaxLength(500);
        builder.Property(u => u.City).HasMaxLength(100);
        builder.Property(u => u.Province).HasMaxLength(100);
        builder.Property(u => u.FacebookUrl).HasMaxLength(500);
        builder.Property(u => u.InstagramUrl).HasMaxLength(500);
        builder.Property(u => u.TotalDistanceKm).HasPrecision(18, 2);
        builder.Property(u => u.RowVersion).IsRowVersion();
        
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => new { u.RoleId, u.IsActive });
        
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Description).HasMaxLength(200);
    }
}

public class MotorcycleConfiguration : IEntityTypeConfiguration<Motorcycle>
{
    public void Configure(EntityTypeBuilder<Motorcycle> builder)
    {
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Brand).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Model).HasMaxLength(100).IsRequired();
        builder.Property(m => m.PlateNumber).HasMaxLength(20);
        builder.Property(m => m.Color).HasMaxLength(50);
        builder.Property(m => m.PhotoUrl).HasMaxLength(500);
        builder.Property(m => m.RowVersion).IsRowVersion();
        
        builder.HasIndex(m => new { m.UserId, m.IsPrimary });
        
        builder.HasOne(m => m.User)
            .WithMany(u => u.Motorcycles)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
