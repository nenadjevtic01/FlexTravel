using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;

namespace FlexTravel.DataAccess
{
    public class FlexTravelContext : DbContext
    {
        public FlexTravelContext(DbContextOptions<FlexTravelContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<Entity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Added && entry.Entity.CreatedAt == default)
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }

        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<Accommodation> Accommodation { get; set; }
        public DbSet<RoomPrice> RoomPrice { get; set; }
        public DbSet<AccommodationType> AccommodationsType { get; set; }
        public DbSet<Agency> Agency { get; set; }
        public DbSet<Attraction> Attraction { get; set; }
        public DbSet<AttractionCategory> AttractionCategory { get; set; }
        public DbSet<AttractionImage> AttractionImage { get; set; }
        public DbSet<Climate> Climate { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Destination> Destination { get; set; }
        public DbSet<DestinationImage> DestinationImage { get; set; }
        public DbSet<Itinerary> Itinerary { get; set; }
        public DbSet<ItineraryActivity> ItineraryActivity { get; set; }
        public DbSet<Reservation> Reservation { get; set; }
        public DbSet<ReservationRoom> ReservationRoom { get; set; }
        public DbSet<ReservationStatusChange> ReservationStatusChange { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<RoomType> RoomType { get; set; }
        public DbSet<Safety> Safety { get; set; }
        public DbSet<Transport> Transport { get; set; }
        public DbSet<Trip> Trip { get; set; }
        public DbSet<TripAccommodation> TripAccommodation { get; set; }
        public DbSet<TripAttraction> TripAttraction { get; set; }
        public DbSet<TripPrice> TripPrice { get; set; }
        public DbSet<UseCase> UseCase { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserFavorites> UserFavorite { get; set; }
        public DbSet<UserUseCase> UserUseCase { get; set; }
        public DbSet<Visa> Visa { get; set; }
        public DbSet<Continent> Continent { get; set; }
        public DbSet<Review> Review { get; set; }
        public DbSet<WaitlistEntry> WaitlistEntry { get; set; }
        public DbSet<PromoCode> PromoCode { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<ReservationTraveler> ReservationTraveler { get; set; }
        public DbSet<Payment> Payment { get; set; }
        public DbSet<ReservationMessage> ReservationMessage { get; set; }
        public DbSet<TripSeries> TripSeries { get; set; }
        public DbSet<TripExtra> TripExtra { get; set; }
        public DbSet<ReservationExtra> ReservationExtra { get; set; }
        public DbSet<PriceAlert> PriceAlert { get; set; }
        public DbSet<ExchangeRate> ExchangeRate { get; set; }
        public DbSet<ScheduledJobRun> ScheduledJobRun { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
    }
}
