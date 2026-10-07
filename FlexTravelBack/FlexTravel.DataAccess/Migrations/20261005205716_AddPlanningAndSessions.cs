using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlexTravel.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanningAndSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GuideRating",
                table: "Review",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrganisationRating",
                table: "Review",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ValueRating",
                table: "Review",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChangedByUserId",
                table: "ReservationStatusChange",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "BalanceReminderSentAt",
                table: "Reservation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepositReminderSentAt",
                table: "Reservation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtrasTotal",
                table: "Reservation",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewInvitationSentAt",
                table: "Reservation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Activity",
                table: "ItineraryActivity",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<int>(
                name: "Meals",
                table: "ItineraryActivity",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Overnight",
                table: "ItineraryActivity",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ItineraryActivity",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Agency",
                type: "nvarchar(1500)",
                maxLength: 1500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Agency",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Agency",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Website",
                table: "Agency",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExchangeRate",
                columns: table => new
                {
                    ExchangeRateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    RatePerEuro = table.Column<decimal>(type: "decimal(12,4)", precision: 12, scale: 4, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRate", x => x.ExchangeRateId);
                    table.ForeignKey(
                        name: "FK_ExchangeRate_User_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PriceAlert",
                columns: table => new
                {
                    PriceAlertId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    MaxPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastNotifiedPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    LastNotifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceAlert", x => x.PriceAlertId);
                    table.ForeignKey(
                        name: "FK_PriceAlert_Destination_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destination",
                        principalColumn: "DestinationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PriceAlert_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WasReplaced = table.Column<bool>(type: "bit", nullable: false),
                    RememberMe = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScheduledJobRun",
                columns: table => new
                {
                    ScheduledJobRunId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExpiredReservations = table.Column<int>(type: "int", nullable: false),
                    PaymentReminders = table.Column<int>(type: "int", nullable: false),
                    ReviewInvitations = table.Column<int>(type: "int", nullable: false),
                    PriceAlerts = table.Column<int>(type: "int", nullable: false),
                    ClosedWaitlists = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduledJobRun", x => x.ScheduledJobRunId);
                });

            migrationBuilder.CreateTable(
                name: "TripExtra",
                columns: table => new
                {
                    TripExtraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PerPerson = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripExtra", x => x.TripExtraId);
                    table.ForeignKey(
                        name: "FK_TripExtra_Trip_TripId",
                        column: x => x.TripId,
                        principalTable: "Trip",
                        principalColumn: "TripId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationExtra",
                columns: table => new
                {
                    ReservationExtraId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    TripExtraId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationExtra", x => x.ReservationExtraId);
                    table.ForeignKey(
                        name: "FK_ReservationExtra_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationExtra_TripExtra_TripExtraId",
                        column: x => x.TripExtraId,
                        principalTable: "TripExtra",
                        principalColumn: "TripExtraId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ExchangeRate",
                columns: new[] { "ExchangeRateId", "Currency", "RatePerEuro", "UpdatedAt", "UpdatedByUserId" },
                values: new object[] { 1, "RSD", 117.2m, new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null });

            migrationBuilder.InsertData(
                table: "UseCase",
                columns: new[] { "UseCaseId", "UseCaseDescription", "UseCaseName" },
                values: new object[,]
                {
                    { 67, "Weather forecast or last year's weather for a destination", "Get weather" },
                    { 68, "Price alerts of the customer", "Get my price alerts" },
                    { 69, "Set or remove a price alert for a destination", "Set price alert" },
                    { 70, "Rates for showing prices in other currencies", "Get exchange rates" },
                    { 71, "Admin updates an exchange rate", "Set exchange rate" },
                    { 72, "Departures and payment deadlines of the agency by date", "Get agency calendar" },
                    { 73, "Sales, occupancy and destinations of the agency for a year", "Get agency report" },
                    { 74, "Public page of an agency with its trips and reviews", "Get agency page" },
                    { 75, "Agency edits its public description, contacts and logo", "Update agency profile" },
                    { 76, "Admin runs the background jobs now", "Run scheduled jobs" },
                    { 77, "Latest runs of the background jobs", "Get scheduled job runs" }
                });

            // Existing accounts get the new use cases of their role (new accounts get them from the seed).
            // Agency employees also get "Upload image" (53) for their logo.
            migrationBuilder.Sql(@"
INSERT INTO [UserUseCase] ([UserId], [UseCaseId])
SELECT u.[UserId], uc.[UseCaseId]
FROM [User] u CROSS JOIN [UseCase] uc
WHERE NOT EXISTS (SELECT 1 FROM [UserUseCase] x WHERE x.[UserId] = u.[UserId] AND x.[UseCaseId] = uc.[UseCaseId])
  AND (
       (uc.[UseCaseId] IN (67, 70, 74))
    OR (u.[Discriminator] = 'Regular user' AND uc.[UseCaseId] IN (68, 69))
    OR (u.[Discriminator] = 'Agency employee' AND uc.[UseCaseId] IN (53, 72, 73, 75))
    OR (u.[Discriminator] = 'Admin' AND uc.[UseCaseId] IN (71, 76, 77)))");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_GuideRating",
                table: "Review",
                sql: "[GuideRating] IS NULL OR [GuideRating] BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_OrganisationRating",
                table: "Review",
                sql: "[OrganisationRating] IS NULL OR [OrganisationRating] BETWEEN 1 AND 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Review_ValueRating",
                table: "Review",
                sql: "[ValueRating] IS NULL OR [ValueRating] BETWEEN 1 AND 5");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRate_Currency",
                table: "ExchangeRate",
                column: "Currency",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRate_UpdatedByUserId",
                table: "ExchangeRate",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlert_DestinationId",
                table: "PriceAlert",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceAlert_UserId_DestinationId",
                table: "PriceAlert",
                columns: new[] { "UserId", "DestinationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_TokenHash",
                table: "RefreshToken",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationExtra_ReservationId",
                table: "ReservationExtra",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationExtra_TripExtraId",
                table: "ReservationExtra",
                column: "TripExtraId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduledJobRun_StartedAt",
                table: "ScheduledJobRun",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_TripExtra_TripId",
                table: "TripExtra",
                column: "TripId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DELETE FROM [UserUseCase] WHERE [UseCaseId] BETWEEN 67 AND 77;
DELETE uuc FROM [UserUseCase] uuc JOIN [User] u ON u.[UserId] = uuc.[UserId]
WHERE uuc.[UseCaseId] = 53 AND u.[Discriminator] = 'Agency employee';
UPDATE [Reservation] SET [Status] = 3 WHERE [Status] = 5;
DELETE FROM [ReservationStatusChange] WHERE [ChangedByUserId] IS NULL;");

            migrationBuilder.DropTable(
                name: "ExchangeRate");

            migrationBuilder.DropTable(
                name: "PriceAlert");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "ReservationExtra");

            migrationBuilder.DropTable(
                name: "ScheduledJobRun");

            migrationBuilder.DropTable(
                name: "TripExtra");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_GuideRating",
                table: "Review");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_OrganisationRating",
                table: "Review");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Review_ValueRating",
                table: "Review");

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 77);

            migrationBuilder.DropColumn(
                name: "GuideRating",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "OrganisationRating",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ValueRating",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "BalanceReminderSentAt",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DepositReminderSentAt",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ExtrasTotal",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ReviewInvitationSentAt",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Meals",
                table: "ItineraryActivity");

            migrationBuilder.DropColumn(
                name: "Overnight",
                table: "ItineraryActivity");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ItineraryActivity");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Agency");

            migrationBuilder.DropColumn(
                name: "Website",
                table: "Agency");

            migrationBuilder.AlterColumn<int>(
                name: "ChangedByUserId",
                table: "ReservationStatusChange",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Activity",
                table: "ItineraryActivity",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }
    }
}
