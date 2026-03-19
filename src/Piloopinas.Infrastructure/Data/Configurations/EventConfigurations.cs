using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Piloopinas.Domain.Entities;

namespace Piloopinas.Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.Name).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(4000);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.EventType).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Difficulty).HasMaxLength(50).IsRequired();
        builder.Property(e => e.DistanceKm).HasPrecision(18, 2);
        builder.Property(e => e.Route).HasMaxLength(8000);
        builder.Property(e => e.StartLocation).HasMaxLength(500).IsRequired();
        builder.Property(e => e.EndLocation).HasMaxLength(500);
        builder.Property(e => e.Region).HasMaxLength(50);
        builder.Property(e => e.Province).HasMaxLength(100);
        builder.Property(e => e.RegistrationFee).HasPrecision(18, 2);
        builder.Property(e => e.RowVersion).IsRowVersion();
        
        builder.HasIndex(e => new { e.StatusId, e.StartDateTime });
        builder.HasIndex(e => new { e.EventType, e.StatusId });
        builder.HasIndex(e => new { e.Region, e.StatusId });
        
        builder.HasOne(e => e.Status)
            .WithMany(s => s.Events)
            .HasForeignKey(e => e.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class EventStatusConfiguration : IEntityTypeConfiguration<EventStatus>
{
    public void Configure(EntityTypeBuilder<EventStatus> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Description).HasMaxLength(200);
    }
}

public class EventRegistrationConfiguration : IEntityTypeConfiguration<EventRegistration>
{
    public void Configure(EntityTypeBuilder<EventRegistration> builder)
    {
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.RegistrationStatus).HasMaxLength(50).IsRequired();
        builder.Property(r => r.PaymentStatus).HasMaxLength(50);
        builder.Property(r => r.PaymentReference).HasMaxLength(100);
        builder.Property(r => r.AmountPaid).HasPrecision(18, 2);
        builder.Property(r => r.ActualDistanceKm).HasPrecision(18, 2);
        builder.Property(r => r.RowVersion).IsRowVersion();
        
        builder.HasIndex(r => new { r.UserId, r.EventId }).IsUnique();
        builder.HasIndex(r => new { r.EventId, r.RegistrationStatus });
        
        builder.HasOne(r => r.User)
            .WithMany(u => u.EventRegistrations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(r => r.Event)
            .WithMany(e => e.Registrations)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(r => r.Motorcycle)
            .WithMany()
            .HasForeignKey(r => r.MotorcycleId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
