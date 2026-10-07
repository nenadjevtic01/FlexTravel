using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlexTravel.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewsWaitlistOffersNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EarlyBookingPercent",
                table: "Trip",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EarlyBookingUntil",
                table: "Trip",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastMinuteDays",
                table: "Trip",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastMinutePercent",
                table: "Trip",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountTotal",
                table: "Reservation",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Offer",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OfferPercent",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PromoCodeId",
                table: "Reservation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PromoPercent",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Subtotal",
                table: "Reservation",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            // Reservations made before discounts existed: no discount, so the subtotal is the total.
            migrationBuilder.Sql("UPDATE [Reservation] SET [Subtotal] = [TotalPrice]");

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromoCode",
                columns: table => new
                {
                    PromoCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgencyId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DiscountPercent = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaxUses = table.Column<int>(type: "int", nullable: true),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCode", x => x.PromoCodeId);
                    table.ForeignKey(
                        name: "FK_PromoCode_Agency_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agency",
                        principalColumn: "AgencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromoCode_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    AccommodationId = table.Column<int>(type: "int", nullable: false),
                    TripRating = table.Column<int>(type: "int", nullable: false),
                    HotelRating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AgencyReply = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    AgencyReplyAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgencyReplyByUserId = table.Column<int>(type: "int", nullable: true),
                    ModerationNote = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ModeratedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModeratedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.ReviewId);
                    table.CheckConstraint("CK_Review_HotelRating", "[HotelRating] BETWEEN 1 AND 5");
                    table.CheckConstraint("CK_Review_TripRating", "[TripRating] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Review_Accommodation_AccommodationId",
                        column: x => x.AccommodationId,
                        principalTable: "Accommodation",
                        principalColumn: "AccommodationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_Trip_TripId",
                        column: x => x.TripId,
                        principalTable: "Trip",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_User_AgencyReplyByUserId",
                        column: x => x.AgencyReplyByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_User_ModeratedByUserId",
                        column: x => x.ModeratedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Review_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WaitlistEntry",
                columns: table => new
                {
                    WaitlistEntryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Adults = table.Column<int>(type: "int", nullable: false),
                    Children = table.Column<int>(type: "int", nullable: false),
                    NotifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaitlistEntry", x => x.WaitlistEntryId);
                    table.ForeignKey(
                        name: "FK_WaitlistEntry_Trip_TripId",
                        column: x => x.TripId,
                        principalTable: "Trip",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WaitlistEntry_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "UseCase",
                columns: new[] { "UseCaseId", "UseCaseDescription", "UseCaseName" },
                values: new object[,]
                {
                    { 32, "Latest notifications of the signed-in user", "Get my notifications" },
                    { 33, "Mark own notifications as read", "Mark notifications read" },
                    { 34, "Favorite destinations of the customer", "Get my favorites" },
                    { 35, "Add or remove a favorite destination", "Set favorite" },
                    { 36, "Reviews of trips and hotels", "Get reviews" },
                    { 37, "Customer reviews a finished trip", "Create review" },
                    { 38, "Agency replies to a review of its trip", "Reply to review" },
                    { 39, "Hide or show a review", "Moderate review" },
                    { 40, "Wait for seats on a sold-out trip", "Join waitlist" },
                    { 41, "Stop waiting for seats on a trip", "Leave waitlist" },
                    { 42, "Trips the customer is waiting for", "Get my waitlist" },
                    { 43, "Promo codes of the agency with usage", "Get promo codes" },
                    { 44, "Agency issues a promo code", "Create promo code" },
                    { 45, "Agency switches a promo code on or off", "Set promo code active" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_PromoCodeId",
                table: "Reservation",
                column: "PromoCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId_CreatedAt",
                table: "Notification",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PromoCode_AgencyId",
                table: "PromoCode",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCode_Code",
                table: "PromoCode",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCode_CreatedByUserId",
                table: "PromoCode",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AccommodationId",
                table: "Review",
                column: "AccommodationId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_AgencyReplyByUserId",
                table: "Review",
                column: "AgencyReplyByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ModeratedByUserId",
                table: "Review",
                column: "ModeratedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ReservationId",
                table: "Review",
                column: "ReservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Review_TripId",
                table: "Review",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistEntry_TripId_UserId",
                table: "WaitlistEntry",
                columns: new[] { "TripId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_WaitlistEntry_UserId",
                table: "WaitlistEntry",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_PromoCode_PromoCodeId",
                table: "Reservation",
                column: "PromoCodeId",
                principalTable: "PromoCode",
                principalColumn: "PromoCodeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_PromoCode_PromoCodeId",
                table: "Reservation");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PromoCode");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "WaitlistEntry");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_PromoCodeId",
                table: "Reservation");

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 45);

            migrationBuilder.DropColumn(
                name: "EarlyBookingPercent",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "EarlyBookingUntil",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "LastMinuteDays",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "LastMinutePercent",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "DiscountTotal",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Offer",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "OfferPercent",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "PromoCodeId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "PromoPercent",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Subtotal",
                table: "Reservation");
        }
    }
}
