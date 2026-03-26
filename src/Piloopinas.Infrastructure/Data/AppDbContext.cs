using Microsoft.EntityFrameworkCore;
using Lakbay.Domain;
using Lakbay.Domain.Entities;

namespace Lakbay.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Motorcycle> Motorcycles => Set<Motorcycle>();
    public DbSet<Event> Events => Set<Event>();
    public DbSet<EventStatus> EventStatuses => Set<EventStatus>();
    public DbSet<EventRegistration> EventRegistrations => Set<EventRegistration>();
    public DbSet<Badge> Badges => Set<Badge>();
    public DbSet<UserBadge> UserBadges => Set<UserBadge>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserFollow> UserFollows => Set<UserFollow>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // UserFollow configuration
        modelBuilder.Entity<UserFollow>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }).IsUnique();
            entity.HasOne(e => e.Follower).WithMany(u => u.Following).HasForeignKey(e => e.FollowerId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Following).WithMany(u => u.Followers).HasForeignKey(e => e.FollowingId).OnDelete(DeleteBehavior.Restrict);
        });
        
        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = RoleConstants.Admin, Name = "Admin", Description = "System administrator" },
            new Role { Id = RoleConstants.Organizer, Name = "Organizer", Description = "Event organizer" },
            new Role { Id = RoleConstants.Rider, Name = "Rider", Description = "Motorcycle rider" }
        );
        
        // Seed Event Statuses
        modelBuilder.Entity<EventStatus>().HasData(
            new EventStatus { Id = EventStatusConstants.Draft, Name = "Draft", Description = "Event is in draft mode" },
            new EventStatus { Id = EventStatusConstants.Published, Name = "Published", Description = "Event is published" },
            new EventStatus { Id = EventStatusConstants.RegistrationOpen, Name = "Registration Open", Description = "Registration is open" },
            new EventStatus { Id = EventStatusConstants.RegistrationClosed, Name = "Registration Closed", Description = "Registration is closed" },
            new EventStatus { Id = EventStatusConstants.InProgress, Name = "In Progress", Description = "Event is ongoing" },
            new EventStatus { Id = EventStatusConstants.Completed, Name = "Completed", Description = "Event has completed" },
            new EventStatus { Id = EventStatusConstants.Cancelled, Name = "Cancelled", Description = "Event was cancelled" }
        );
        
        // Seed Badges - using static date for deterministic migration
        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Badge>().HasData(
            new Badge { Id = 1, Name = "First Ride", Description = "Completed your first event", IconUrl = "🏍️", Category = "Events", CriteriaType = "EventsCompleted", CriteriaValue = 1, PointsValue = 50, IsActive = true, CreatedAt = seedDate },
            new Badge { Id = 2, Name = "Road Warrior", Description = "Completed 5 events", IconUrl = "⚔️", Category = "Events", CriteriaType = "EventsCompleted", CriteriaValue = 5, PointsValue = 150, IsActive = true, CreatedAt = seedDate },
            new Badge { Id = 3, Name = "Century Rider", Description = "Reached 100km total distance", IconUrl = "💯", Category = "Distance", CriteriaType = "DistanceReached", CriteriaValue = 100, PointsValue = 100, IsActive = true, CreatedAt = seedDate },
            new Badge { Id = 4, Name = "1K Club", Description = "Reached 1,000km total distance", IconUrl = "🎯", Category = "Distance", CriteriaType = "DistanceReached", CriteriaValue = 1000, PointsValue = 500, IsActive = true, CreatedAt = seedDate },
            new Badge { Id = 5, Name = "Loop Master", Description = "Completed a Philippine Loop event", IconUrl = "🏝️", Category = "Special", CriteriaType = "EventTypeCompleted", CriteriaValue = null, PointsValue = 1000, IsActive = true, CreatedAt = seedDate }
        );
    }
}
