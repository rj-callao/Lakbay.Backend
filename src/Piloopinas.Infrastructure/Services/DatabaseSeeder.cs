using Microsoft.EntityFrameworkCore;
using Piloopinas.Domain;
using Piloopinas.Domain.Entities;
using Piloopinas.Infrastructure.Data;

namespace Piloopinas.Infrastructure.Services;

public class DatabaseSeeder
{
    private readonly AppDbContext _context;

    public DatabaseSeeder(AppDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Seed admin user
        if (!await _context.Users.AnyAsync(u => u.Email == "admin@piloopinas.com"))
        {
            _context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@piloopinas.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                FirstName = "System",
                LastName = "Administrator",
                RoleId = RoleConstants.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            });
        }

        // Seed riders
        if (!await _context.Users.AnyAsync(u => u.RoleId == RoleConstants.Rider))
        {
            var riders = new[]
            {
                new User { Id = Guid.NewGuid(), Email = "juan@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rider@123"), FirstName = "Juan", LastName = "Dela Cruz", Province = "Metro Manila", RoleId = RoleConstants.Rider, TotalDistanceKm = 4800, TotalEventsCompleted = 22, TotalPoints = 5200, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new User { Id = Guid.NewGuid(), Email = "maria@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rider@123"), FirstName = "Maria", LastName = "Santos", Province = "Cebu", RoleId = RoleConstants.Rider, TotalDistanceKm = 6500, TotalEventsCompleted = 30, TotalPoints = 7800, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new User { Id = Guid.NewGuid(), Email = "pedro@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rider@123"), FirstName = "Pedro", LastName = "Reyes", Province = "Davao del Sur", RoleId = RoleConstants.Rider, TotalDistanceKm = 9200, TotalEventsCompleted = 42, TotalPoints = 12500, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new User { Id = Guid.NewGuid(), Email = "ana@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rider@123"), FirstName = "Ana", LastName = "Garcia", Province = "Pampanga", RoleId = RoleConstants.Rider, TotalDistanceKm = 3200, TotalEventsCompleted = 14, TotalPoints = 3400, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new User { Id = Guid.NewGuid(), Email = "carlos@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rider@123"), FirstName = "Carlos", LastName = "Mendoza", Province = "Iloilo", RoleId = RoleConstants.Rider, TotalDistanceKm = 7100, TotalEventsCompleted = 35, TotalPoints = 8900, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new User { Id = Guid.NewGuid(), Email = "rosa@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rider@123"), FirstName = "Rosa", LastName = "Villanueva", Province = "Batangas", RoleId = RoleConstants.Rider, TotalDistanceKm = 2100, TotalEventsCompleted = 10, TotalPoints = 2200, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new User { Id = Guid.NewGuid(), Email = "miguel@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rider@123"), FirstName = "Miguel", LastName = "Torres", Province = "Laguna", RoleId = RoleConstants.Rider, TotalDistanceKm = 5500, TotalEventsCompleted = 25, TotalPoints = 6100, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new User { Id = Guid.NewGuid(), Email = "elena@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Rider@123"), FirstName = "Elena", LastName = "Ramos", Province = "Bukidnon", RoleId = RoleConstants.Rider, TotalDistanceKm = 4000, TotalEventsCompleted = 18, TotalPoints = 4500, IsActive = true, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
            };

            _context.Users.AddRange(riders);
            await _context.SaveChangesAsync();

            // Seed motorcycles for riders
            var savedRiders = await _context.Users.Where(u => u.RoleId == RoleConstants.Rider).ToListAsync();
            var motorcycleData = new (string Brand, string Model, int Year, int Cc, string Color)[]
            {
                ("Honda", "XR 150L", 2024, 150, "Red"),
                ("Yamaha", "MT-15", 2023, 155, "Blue"),
                ("Kawasaki", "Ninja 400", 2024, 399, "Green"),
                ("Honda", "CRF 300L", 2023, 286, "Red"),
                ("Suzuki", "V-Strom 650", 2024, 645, "Black"),
                ("Royal Enfield", "Himalayan 450", 2024, 452, "Gray"),
                ("BMW", "G 310 GS", 2023, 313, "White"),
                ("KTM", "390 Adventure", 2024, 373, "Orange"),
            };

            for (int i = 0; i < savedRiders.Count && i < motorcycleData.Length; i++)
            {
                var m = motorcycleData[i];
                _context.Motorcycles.Add(new Motorcycle
                {
                    Id = Guid.NewGuid(),
                    UserId = savedRiders[i].Id,
                    Brand = m.Brand,
                    Model = m.Model,
                    Year = m.Year,
                    EngineDisplacementCc = m.Cc,
                    Color = m.Color,
                    IsPrimary = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }
        }

        // Badges are seeded via HasData in AppDbContext - skip here

        // Seed events
        if (!await _context.Events.AnyAsync())
        {
            var events = new[]
            {
                new Event { Id = Guid.NewGuid(), Name = "Manila to Baguio Endurance Challenge", Description = "A challenging ride from Manila to the Summer Capital of the Philippines. Experience the zigzag roads of Kennon Road!", EventType = "Endurance", Difficulty = "Hard", DistanceKm = 250, StartLocation = "SM Mall of Asia, Pasay City", EndLocation = "Burnham Park, Baguio City", Region = "Luzon", Province = "Benguet", StartDateTime = DateTime.UtcNow.AddDays(14), EndDateTime = DateTime.UtcNow.AddDays(14).AddHours(12), RegistrationDeadline = DateTime.UtcNow.AddDays(10), MaxParticipants = 100, RegistrationFee = 1500, PointsReward = 250, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Visayan Loop Challenge", Description = "Complete a loop around Cebu Island! See beautiful beaches and mountain views.", EventType = "Loop", Difficulty = "Moderate", DistanceKm = 350, StartLocation = "Cebu City", EndLocation = "Cebu City", Region = "Visayas", Province = "Cebu", StartDateTime = DateTime.UtcNow.AddDays(21), EndDateTime = DateTime.UtcNow.AddDays(22), RegistrationDeadline = DateTime.UtcNow.AddDays(17), MaxParticipants = 75, RegistrationFee = 2000, PointsReward = 350, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Philippine Loop - Luzon Edition", Description = "The ultimate challenge: Complete the entire Luzon loop! 7-day adventure around the largest island.", EventType = "Loop", Difficulty = "Extreme", DistanceKm = 2500, StartLocation = "Manila", EndLocation = "Manila", Region = "Luzon", Province = "Metro Manila", StartDateTime = DateTime.UtcNow.AddDays(45), EndDateTime = DateTime.UtcNow.AddDays(52), RegistrationDeadline = DateTime.UtcNow.AddDays(30), MaxParticipants = 50, RegistrationFee = 10000, PointsReward = 2500, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Tagaytay Sunrise Ride", Description = "A beginner-friendly ride to catch the sunrise at Tagaytay. Perfect for new riders!", EventType = "Tour", Difficulty = "Easy", DistanceKm = 120, StartLocation = "Alabang Town Center", EndLocation = "Tagaytay Picnic Grove", Region = "Luzon", Province = "Cavite", StartDateTime = DateTime.UtcNow.AddDays(7), EndDateTime = DateTime.UtcNow.AddDays(7).AddHours(8), RegistrationDeadline = DateTime.UtcNow.AddDays(5), MaxParticipants = 50, RegistrationFee = 500, PointsReward = 100, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Davao-GenSan Coastal Run", Description = "Ride along the stunning coastline of Southern Mindanao. Enjoy the ocean breeze and scenic views!", EventType = "Endurance", Difficulty = "Moderate", DistanceKm = 180, StartLocation = "Davao City Hall", EndLocation = "General Santos City", Region = "Mindanao", Province = "Davao del Sur", StartDateTime = DateTime.UtcNow.AddDays(10), EndDateTime = DateTime.UtcNow.AddDays(10).AddHours(10), RegistrationDeadline = DateTime.UtcNow.AddDays(7), MaxParticipants = 80, RegistrationFee = 1200, PointsReward = 200, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Cordillera Mountain Trail", Description = "Conquer the mountains of the Cordillera. Not for the faint-hearted!", EventType = "Endurance", Difficulty = "Extreme", DistanceKm = 400, StartLocation = "Baguio City", EndLocation = "Sagada, Mountain Province", Region = "Luzon", Province = "Mountain Province", StartDateTime = DateTime.UtcNow.AddDays(28), EndDateTime = DateTime.UtcNow.AddDays(29), RegistrationDeadline = DateTime.UtcNow.AddDays(21), MaxParticipants = 40, RegistrationFee = 3500, PointsReward = 500, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Ilocos Heritage Tour", Description = "A scenic tour through the heritage sites of Ilocos. Visit Vigan, Pagudpud, and Bangui Windmills.", EventType = "Tour", Difficulty = "Moderate", DistanceKm = 500, StartLocation = "San Fernando, La Union", EndLocation = "Laoag City", Region = "Luzon", Province = "Ilocos Norte", StartDateTime = DateTime.UtcNow.AddDays(35), EndDateTime = DateTime.UtcNow.AddDays(37), RegistrationDeadline = DateTime.UtcNow.AddDays(28), MaxParticipants = 60, RegistrationFee = 2500, PointsReward = 400, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Palawan Paradise Ride", Description = "Explore the last frontier! Ride through El Nido and Puerto Princesa.", EventType = "Tour", Difficulty = "Hard", DistanceKm = 600, StartLocation = "Puerto Princesa Airport", EndLocation = "El Nido", Region = "Luzon", Province = "Palawan", StartDateTime = DateTime.UtcNow.AddDays(60), EndDateTime = DateTime.UtcNow.AddDays(63), RegistrationDeadline = DateTime.UtcNow.AddDays(45), MaxParticipants = 30, RegistrationFee = 5000, PointsReward = 600, StatusId = EventStatusConstants.Published, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Batangas Lakeside Loop", Description = "A relaxing loop ride around Taal Lake with stops at local eateries.", EventType = "Loop", Difficulty = "Easy", DistanceKm = 90, StartLocation = "Lipa City", EndLocation = "Lipa City", Region = "Luzon", Province = "Batangas", StartDateTime = DateTime.UtcNow.AddDays(5), EndDateTime = DateTime.UtcNow.AddDays(5).AddHours(6), RegistrationDeadline = DateTime.UtcNow.AddDays(3), MaxParticipants = 40, RegistrationFee = 350, PointsReward = 75, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Leyte-Samar Bridge Run", Description = "Cross the iconic San Juanico Bridge and explore Eastern Visayas.", EventType = "Endurance", Difficulty = "Hard", DistanceKm = 300, StartLocation = "Tacloban City", EndLocation = "Catbalogan, Samar", Region = "Visayas", Province = "Leyte", StartDateTime = DateTime.UtcNow.AddDays(40), EndDateTime = DateTime.UtcNow.AddDays(41), RegistrationDeadline = DateTime.UtcNow.AddDays(30), MaxParticipants = 55, RegistrationFee = 2200, PointsReward = 350, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },

                // Additional events for richer seed data
                new Event { Id = Guid.NewGuid(), Name = "Zambales Coastal Cruise", Description = "Cruise along the beautiful west coast of Zambales with beach stops at Crystal Beach, Nagsasa Cove, and Anawangin.", EventType = "Tour", Difficulty = "Easy", DistanceKm = 160, StartLocation = "Subic Bay Freeport", EndLocation = "Iba, Zambales", Region = "Luzon", Province = "Zambales", StartDateTime = DateTime.UtcNow.AddDays(8), EndDateTime = DateTime.UtcNow.AddDays(8).AddHours(9), RegistrationDeadline = DateTime.UtcNow.AddDays(6), MaxParticipants = 60, RegistrationFee = 800, PointsReward = 120, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Negros Island Adventure", Description = "Explore the sugar capital of the Philippines! From Bacolod to Dumaguete with mountain and ocean views.", EventType = "Endurance", Difficulty = "Hard", DistanceKm = 350, StartLocation = "Bacolod City", EndLocation = "Dumaguete City", Region = "Visayas", Province = "Negros Occidental", StartDateTime = DateTime.UtcNow.AddDays(18), EndDateTime = DateTime.UtcNow.AddDays(19), RegistrationDeadline = DateTime.UtcNow.AddDays(14), MaxParticipants = 45, RegistrationFee = 2800, PointsReward = 380, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Mt. Pinatubo Rim Ride", Description = "Ride through the lahar fields and witness the majestic crater of Mt. Pinatubo. An unforgettable off-road experience!", EventType = "Endurance", Difficulty = "Extreme", DistanceKm = 200, StartLocation = "Capas, Tarlac", EndLocation = "Botolan, Zambales", Region = "Luzon", Province = "Tarlac", StartDateTime = DateTime.UtcNow.AddDays(25), EndDateTime = DateTime.UtcNow.AddDays(25).AddHours(14), RegistrationDeadline = DateTime.UtcNow.AddDays(20), MaxParticipants = 30, RegistrationFee = 4000, PointsReward = 550, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Cagayan Valley Express", Description = "Speed through the vast plains of Cagayan Valley. Flat roads, big skies, and rice paddy scenery for days.", EventType = "Endurance", Difficulty = "Moderate", DistanceKm = 480, StartLocation = "Santiago City, Isabela", EndLocation = "Tuguegarao City", Region = "Luzon", Province = "Isabela", StartDateTime = DateTime.UtcNow.AddDays(32), EndDateTime = DateTime.UtcNow.AddDays(33), RegistrationDeadline = DateTime.UtcNow.AddDays(25), MaxParticipants = 70, RegistrationFee = 1800, PointsReward = 300, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Bohol Countryside Loop", Description = "See the Chocolate Hills, tarsiers, and stunning countryside churches on this scenic Bohol loop.", EventType = "Loop", Difficulty = "Easy", DistanceKm = 130, StartLocation = "Tagbilaran City", EndLocation = "Tagbilaran City", Region = "Visayas", Province = "Bohol", StartDateTime = DateTime.UtcNow.AddDays(12), EndDateTime = DateTime.UtcNow.AddDays(12).AddHours(7), RegistrationDeadline = DateTime.UtcNow.AddDays(9), MaxParticipants = 50, RegistrationFee = 600, PointsReward = 85, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Bukidnon Highland Rally", Description = "Navigate the twisty mountain roads of Bukidnon at altitude. Cool weather and pine-forested trails await.", EventType = "Endurance", Difficulty = "Hard", DistanceKm = 280, StartLocation = "Cagayan de Oro", EndLocation = "Malaybalay City", Region = "Mindanao", Province = "Bukidnon", StartDateTime = DateTime.UtcNow.AddDays(22), EndDateTime = DateTime.UtcNow.AddDays(22).AddHours(12), RegistrationDeadline = DateTime.UtcNow.AddDays(18), MaxParticipants = 40, RegistrationFee = 2500, PointsReward = 320, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Siargao Island Explorer", Description = "Ride around the surfing capital of the Philippines! Cloud 9, Magpupungko Rock Pools, and island vibes.", EventType = "Tour", Difficulty = "Moderate", DistanceKm = 100, StartLocation = "General Luna, Siargao", EndLocation = "Dapa, Siargao", Region = "Mindanao", Province = "Surigao del Norte", StartDateTime = DateTime.UtcNow.AddDays(50), EndDateTime = DateTime.UtcNow.AddDays(50).AddHours(8), RegistrationDeadline = DateTime.UtcNow.AddDays(40), MaxParticipants = 35, RegistrationFee = 1500, PointsReward = 180, StatusId = EventStatusConstants.Published, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Quezon Province Coconut Trail", Description = "Ride through endless coconut plantations in Quezon Province. Lucban, Tayabas, and Padre Burgos await!", EventType = "Tour", Difficulty = "Easy", DistanceKm = 200, StartLocation = "Lucena City", EndLocation = "Padre Burgos, Quezon", Region = "Luzon", Province = "Quezon", StartDateTime = DateTime.UtcNow.AddDays(9), EndDateTime = DateTime.UtcNow.AddDays(9).AddHours(10), RegistrationDeadline = DateTime.UtcNow.AddDays(7), MaxParticipants = 55, RegistrationFee = 700, PointsReward = 110, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Mindanao Grand Loop", Description = "The ultimate Mindanao experience! A 5-day loop covering Davao, GenSan, Cotabato, CDO, and back.", EventType = "Loop", Difficulty = "Extreme", DistanceKm = 1800, StartLocation = "Davao City", EndLocation = "Davao City", Region = "Mindanao", Province = "Davao del Sur", StartDateTime = DateTime.UtcNow.AddDays(70), EndDateTime = DateTime.UtcNow.AddDays(75), RegistrationDeadline = DateTime.UtcNow.AddDays(55), MaxParticipants = 25, RegistrationFee = 8000, PointsReward = 2000, StatusId = EventStatusConstants.Published, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Bicol Express Ride", Description = "Follow the famous Bicol Express route! Ride from Naga to Legazpi with Mayon Volcano as your backdrop.", EventType = "Endurance", Difficulty = "Moderate", DistanceKm = 220, StartLocation = "Naga City", EndLocation = "Legazpi City", Region = "Luzon", Province = "Albay", StartDateTime = DateTime.UtcNow.AddDays(16), EndDateTime = DateTime.UtcNow.AddDays(16).AddHours(11), RegistrationDeadline = DateTime.UtcNow.AddDays(12), MaxParticipants = 65, RegistrationFee = 1400, PointsReward = 220, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Panay Island Perimeter Run", Description = "Circle the entire island of Panay! Experience Iloilo, Antique, Aklan, and Capiz in one epic ride.", EventType = "Loop", Difficulty = "Hard", DistanceKm = 550, StartLocation = "Iloilo City", EndLocation = "Iloilo City", Region = "Visayas", Province = "Iloilo", StartDateTime = DateTime.UtcNow.AddDays(38), EndDateTime = DateTime.UtcNow.AddDays(40), RegistrationDeadline = DateTime.UtcNow.AddDays(30), MaxParticipants = 35, RegistrationFee = 3200, PointsReward = 450, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Aurora Pacific Coast Ride", Description = "Ride along the Pacific coast of Aurora province. Baler surfing spots, Ditumabo Falls, and raw coastal beauty.", EventType = "Tour", Difficulty = "Moderate", DistanceKm = 240, StartLocation = "Cabanatuan City", EndLocation = "Baler, Aurora", Region = "Luzon", Province = "Aurora", StartDateTime = DateTime.UtcNow.AddDays(15), EndDateTime = DateTime.UtcNow.AddDays(15).AddHours(12), RegistrationDeadline = DateTime.UtcNow.AddDays(11), MaxParticipants = 45, RegistrationFee = 1600, PointsReward = 230, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "Zamboanga Peninsula Dash", Description = "Ride to the southernmost tip of Western Mindanao and experience the vibrant culture of Zamboanga City.", EventType = "Endurance", Difficulty = "Hard", DistanceKm = 320, StartLocation = "Pagadian City", EndLocation = "Zamboanga City", Region = "Mindanao", Province = "Zamboanga del Sur", StartDateTime = DateTime.UtcNow.AddDays(48), EndDateTime = DateTime.UtcNow.AddDays(49), RegistrationDeadline = DateTime.UtcNow.AddDays(38), MaxParticipants = 30, RegistrationFee = 2600, PointsReward = 370, StatusId = EventStatusConstants.Published, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
                new Event { Id = Guid.NewGuid(), Name = "La Mesa Eco Trail Sprint", Description = "A quick urban sprint around the La Mesa Eco Park reserve. Great for a weekend morning warm-up ride.", EventType = "Tour", Difficulty = "Easy", DistanceKm = 45, StartLocation = "Quezon City Circle", EndLocation = "La Mesa Eco Park", Region = "Luzon", Province = "Metro Manila", StartDateTime = DateTime.UtcNow.AddDays(3), EndDateTime = DateTime.UtcNow.AddDays(3).AddHours(4), RegistrationDeadline = DateTime.UtcNow.AddDays(2), MaxParticipants = 30, RegistrationFee = 250, PointsReward = 40, StatusId = EventStatusConstants.RegistrationOpen, CreatedAt = DateTime.UtcNow, CreatedBy = "System" },
            };

            _context.Events.AddRange(events);
        }

        await _context.SaveChangesAsync();

        // Seed event registrations (link riders to events)
        if (!await _context.EventRegistrations.AnyAsync())
        {
            var riders = await _context.Users.Where(u => u.RoleId == RoleConstants.Rider).ToListAsync();
            var allEvents = await _context.Events.ToListAsync();
            var motorcycles = await _context.Motorcycles.ToListAsync();

            if (riders.Count > 0 && allEvents.Count > 0)
            {
                var registrations = new List<EventRegistration>();
                var random = new Random(42); // deterministic seed

                // Register riders for ALL upcoming events (not just first 6)
                // so "My Upcoming Events" section has data for every rider
                foreach (var ev in allEvents.Where(e => e.StartDateTime > DateTime.UtcNow))
                {
                    var numRegistrants = Math.Min(random.Next(3, 6), riders.Count);
                    var registeredRiders = riders.OrderBy(_ => random.Next()).Take(numRegistrants).ToList();

                    foreach (var rider in registeredRiders)
                    {
                        var motorcycle = motorcycles.FirstOrDefault(m => m.UserId == rider.Id);
                        registrations.Add(new EventRegistration
                        {
                            Id = Guid.NewGuid(),
                            UserId = rider.Id,
                            EventId = ev.Id,
                            MotorcycleId = motorcycle?.Id,
                            RegistrationStatus = "Confirmed",
                            PaymentStatus = "Paid",
                            AmountPaid = ev.RegistrationFee,
                            RegisteredAt = DateTime.UtcNow.AddDays(-random.Next(1, 10)),
                            CreatedAt = DateTime.UtcNow,
                            CreatedBy = "System"
                        });
                    }
                }

                _context.EventRegistrations.AddRange(registrations);
            }
        }

        // Seed user badges for top riders
        if (!await _context.UserBadges.AnyAsync())
        {
            var badges = await _context.Set<Badge>().ToListAsync();
            var topRiders = await _context.Users
                .Where(u => u.RoleId == RoleConstants.Rider && u.TotalPoints > 3000)
                .ToListAsync();

            if (badges.Count > 0 && topRiders.Count > 0)
            {
                var userBadges = new List<UserBadge>();
                foreach (var rider in topRiders)
                {
                    // Give top riders some badges
                    var badgesToAward = badges.Take(Math.Min(5, badges.Count));
                    foreach (var badge in badgesToAward)
                    {
                        userBadges.Add(new UserBadge
                        {
                            Id = Guid.NewGuid(),
                            UserId = rider.Id,
                            BadgeId = badge.Id,
                            EarnedAt = DateTime.UtcNow.AddDays(-new Random().Next(1, 60))
                        });
                    }
                }
                _context.UserBadges.AddRange(userBadges);
            }
        }

        await _context.SaveChangesAsync();
    }
}
