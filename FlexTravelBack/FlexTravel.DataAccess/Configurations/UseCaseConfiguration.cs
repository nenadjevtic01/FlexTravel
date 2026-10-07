using FlexTravel.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlexTravel.DataAccess.Configurations
{
    public class UseCaseConfiguration : IEntityTypeConfiguration<UseCase>
    {
        public void Configure(EntityTypeBuilder<UseCase> builder)
        {
            builder.Property(x => x.UseCaseId).ValueGeneratedNever();
            builder.Property(x => x.UseCaseName).IsRequired().HasMaxLength(50);
            builder.Property(x => x.UseCaseDescription).IsRequired().HasMaxLength(200);

            builder
                .HasMany(x => x.Users)
                .WithOne(x => x.UseCase)
                .HasForeignKey(x => x.UseCaseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new UseCase { UseCaseId = 1, UseCaseName = "Fill database", UseCaseDescription = "Initial fill database" },
                new UseCase { UseCaseId = 2, UseCaseName = "Get continents", UseCaseDescription = "List of continents" },
                new UseCase { UseCaseId = 3, UseCaseName = "Get countries", UseCaseDescription = "List of countries with visa and safety info" },
                new UseCase { UseCaseId = 4, UseCaseName = "Search destinations", UseCaseDescription = "Search destinations by keyword, continent and country" },
                new UseCase { UseCaseId = 5, UseCaseName = "Get destination", UseCaseDescription = "Destination details with country info and accommodations" },
                new UseCase { UseCaseId = 6, UseCaseName = "Search audit logs", UseCaseDescription = "Search the log of executed use cases" },
                new UseCase { UseCaseId = 7, UseCaseName = "Search trips", UseCaseDescription = "Search upcoming trips" },
                new UseCase { UseCaseId = 8, UseCaseName = "Get trip", UseCaseDescription = "Trip details with itinerary, hotels and availability" },
                new UseCase { UseCaseId = 9, UseCaseName = "Calculate price", UseCaseDescription = "Price quote for a booking without saving it" },
                new UseCase { UseCaseId = 10, UseCaseName = "Register user", UseCaseDescription = "Create a customer account" },
                new UseCase { UseCaseId = 11, UseCaseName = "Create reservation", UseCaseDescription = "Book a trip" },
                new UseCase { UseCaseId = 12, UseCaseName = "Get my reservations", UseCaseDescription = "Reservations of the signed-in customer" },
                new UseCase { UseCaseId = 13, UseCaseName = "Cancel reservation", UseCaseDescription = "Customer cancels own reservation" },
                new UseCase { UseCaseId = 14, UseCaseName = "Get agency trips", UseCaseDescription = "Trips of the employee's agency with occupancy" },
                new UseCase { UseCaseId = 15, UseCaseName = "Create trip", UseCaseDescription = "Agency publishes a new trip" },
                new UseCase { UseCaseId = 16, UseCaseName = "Deactivate trip", UseCaseDescription = "Agency withdraws a trip" },
                new UseCase { UseCaseId = 17, UseCaseName = "Get agency reservations", UseCaseDescription = "Reservations for the agency's trips" },
                new UseCase { UseCaseId = 18, UseCaseName = "Decide reservation", UseCaseDescription = "Agency confirms or rejects a reservation" },
                new UseCase { UseCaseId = 19, UseCaseName = "Get trip form options", UseCaseDescription = "Transports, destinations and hotels for the trip form" },
                new UseCase { UseCaseId = 20, UseCaseName = "Get trip destinations", UseCaseDescription = "Destinations that have upcoming trips" },
                new UseCase { UseCaseId = 21, UseCaseName = "Get reservation", UseCaseDescription = "Reservation details with price breakdown and history" },
                new UseCase { UseCaseId = 22, UseCaseName = "Update trip", UseCaseDescription = "Agency edits its trip" },
                new UseCase { UseCaseId = 23, UseCaseName = "Get trip for edit", UseCaseDescription = "Trip data for the edit form" },
                new UseCase { UseCaseId = 24, UseCaseName = "Get admin stats", UseCaseDescription = "System overview for the admin" },
                new UseCase { UseCaseId = 25, UseCaseName = "Search users", UseCaseDescription = "Search user accounts" },
                new UseCase { UseCaseId = 26, UseCaseName = "Get user", UseCaseDescription = "User account with permissions" },
                new UseCase { UseCaseId = 27, UseCaseName = "Update user use cases", UseCaseDescription = "Grant or revoke use cases of a user" },
                new UseCase { UseCaseId = 28, UseCaseName = "Set user active", UseCaseDescription = "Block or unblock a user account" },
                new UseCase { UseCaseId = 29, UseCaseName = "Get agencies", UseCaseDescription = "Agencies with employees and results" },
                new UseCase { UseCaseId = 30, UseCaseName = "Create agency", UseCaseDescription = "Register a new partner agency" },
                new UseCase { UseCaseId = 31, UseCaseName = "Create agency employee", UseCaseDescription = "Create an account for an agency employee" },
                new UseCase { UseCaseId = 32, UseCaseName = "Get my notifications", UseCaseDescription = "Latest notifications of the signed-in user" },
                new UseCase { UseCaseId = 33, UseCaseName = "Mark notifications read", UseCaseDescription = "Mark own notifications as read" },
                new UseCase { UseCaseId = 34, UseCaseName = "Get my favorites", UseCaseDescription = "Favorite destinations of the customer" },
                new UseCase { UseCaseId = 35, UseCaseName = "Set favorite", UseCaseDescription = "Add or remove a favorite destination" },
                new UseCase { UseCaseId = 36, UseCaseName = "Get reviews", UseCaseDescription = "Reviews of trips and hotels" },
                new UseCase { UseCaseId = 37, UseCaseName = "Create review", UseCaseDescription = "Customer reviews a finished trip" },
                new UseCase { UseCaseId = 38, UseCaseName = "Reply to review", UseCaseDescription = "Agency replies to a review of its trip" },
                new UseCase { UseCaseId = 39, UseCaseName = "Moderate review", UseCaseDescription = "Hide or show a review" },
                new UseCase { UseCaseId = 40, UseCaseName = "Join waitlist", UseCaseDescription = "Wait for seats on a sold-out trip" },
                new UseCase { UseCaseId = 41, UseCaseName = "Leave waitlist", UseCaseDescription = "Stop waiting for seats on a trip" },
                new UseCase { UseCaseId = 42, UseCaseName = "Get my waitlist", UseCaseDescription = "Trips the customer is waiting for" },
                new UseCase { UseCaseId = 43, UseCaseName = "Get promo codes", UseCaseDescription = "Promo codes of the agency with usage" },
                new UseCase { UseCaseId = 44, UseCaseName = "Create promo code", UseCaseDescription = "Agency issues a promo code" },
                new UseCase { UseCaseId = 45, UseCaseName = "Set promo code active", UseCaseDescription = "Agency switches a promo code on or off" },
                new UseCase { UseCaseId = 46, UseCaseName = "Save country", UseCaseDescription = "Add or edit a country with visa and safety info" },
                new UseCase { UseCaseId = 47, UseCaseName = "Save destination", UseCaseDescription = "Add or edit a destination with climate and images" },
                new UseCase { UseCaseId = 48, UseCaseName = "Get destination for edit", UseCaseDescription = "Destination with hotels, rooms, prices and sights for the catalog editor" },
                new UseCase { UseCaseId = 49, UseCaseName = "Save hotel", UseCaseDescription = "Add or edit a hotel with room categories and prices" },
                new UseCase { UseCaseId = 50, UseCaseName = "Save sight", UseCaseDescription = "Add or edit a sight of a destination" },
                new UseCase { UseCaseId = 51, UseCaseName = "Delete sight", UseCaseDescription = "Remove a sight that no trip uses" },
                new UseCase { UseCaseId = 52, UseCaseName = "Get catalog options", UseCaseDescription = "Continents, hotel types, room types and sight categories" },
                new UseCase { UseCaseId = 53, UseCaseName = "Upload image", UseCaseDescription = "Upload a destination image" },
                new UseCase { UseCaseId = 54, UseCaseName = "Get my profile", UseCaseDescription = "Account details of the signed-in user" },
                new UseCase { UseCaseId = 55, UseCaseName = "Update my profile", UseCaseDescription = "Change own name, phone and address" },
                new UseCase { UseCaseId = 56, UseCaseName = "Change password", UseCaseDescription = "Change own password" },
                new UseCase { UseCaseId = 57, UseCaseName = "Update travelers", UseCaseDescription = "Correct the names of travelers on a reservation" },
                new UseCase { UseCaseId = 58, UseCaseName = "Get trip manifest", UseCaseDescription = "Passenger list of a departure" },
                new UseCase { UseCaseId = 59, UseCaseName = "Record payment", UseCaseDescription = "Agency records a payment or refund" },
                new UseCase { UseCaseId = 60, UseCaseName = "Get reservation messages", UseCaseDescription = "Messages between the customer and the agency" },
                new UseCase { UseCaseId = 61, UseCaseName = "Send reservation message", UseCaseDescription = "Write to the customer or the agency about a reservation" },
                new UseCase { UseCaseId = 62, UseCaseName = "Get similar trips", UseCaseDescription = "Other departures and similar trips" },
                new UseCase { UseCaseId = 63, UseCaseName = "Set destination active", UseCaseDescription = "Hide or show a destination" },
                new UseCase { UseCaseId = 64, UseCaseName = "Set hotel active", UseCaseDescription = "Hide or show a hotel" },
                new UseCase { UseCaseId = 65, UseCaseName = "Clean up images", UseCaseDescription = "Delete uploaded images that nothing uses" },
                new UseCase { UseCaseId = 66, UseCaseName = "Add departure", UseCaseDescription = "Agency adds another date of a trip" },
                new UseCase { UseCaseId = 67, UseCaseName = "Get weather", UseCaseDescription = "Weather forecast or last year's weather for a destination" },
                new UseCase { UseCaseId = 68, UseCaseName = "Get my price alerts", UseCaseDescription = "Price alerts of the customer" },
                new UseCase { UseCaseId = 69, UseCaseName = "Set price alert", UseCaseDescription = "Set or remove a price alert for a destination" },
                new UseCase { UseCaseId = 70, UseCaseName = "Get exchange rates", UseCaseDescription = "Rates for showing prices in other currencies" },
                new UseCase { UseCaseId = 71, UseCaseName = "Set exchange rate", UseCaseDescription = "Admin updates an exchange rate" },
                new UseCase { UseCaseId = 72, UseCaseName = "Get agency calendar", UseCaseDescription = "Departures and payment deadlines of the agency by date" },
                new UseCase { UseCaseId = 73, UseCaseName = "Get agency report", UseCaseDescription = "Sales, occupancy and destinations of the agency for a year" },
                new UseCase { UseCaseId = 74, UseCaseName = "Get agency page", UseCaseDescription = "Public page of an agency with its trips and reviews" },
                new UseCase { UseCaseId = 75, UseCaseName = "Update agency profile", UseCaseDescription = "Agency edits its public description, contacts and logo" },
                new UseCase { UseCaseId = 76, UseCaseName = "Run scheduled jobs", UseCaseDescription = "Admin runs the background jobs now" },
                new UseCase { UseCaseId = 77, UseCaseName = "Get scheduled job runs", UseCaseDescription = "Latest runs of the background jobs" },
                new UseCase { UseCaseId = 78, UseCaseName = "Change my avatar", UseCaseDescription = "Upload or remove own profile photo" },
                new UseCase { UseCaseId = 79, UseCaseName = "Remove user avatar", UseCaseDescription = "Admin removes an unsuitable profile photo" },
                new UseCase { UseCaseId = 80, UseCaseName = "Update user", UseCaseDescription = "Admin corrects a user's name, e-mail, phone and address" });
        }
    }
}
