using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlexTravel.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FailedLoginAttempts",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LockedUntil",
                table: "User",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TripSeriesId",
                table: "Trip",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CancellationFee",
                table: "Reservation",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Destination",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Destination",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Accommodation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedByUserId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payment_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Payment_User_RecordedByUserId",
                        column: x => x.RecordedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationMessage",
                columns: table => new
                {
                    ReservationMessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    SenderUserId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationMessage", x => x.ReservationMessageId);
                    table.ForeignKey(
                        name: "FK_ReservationMessage_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationMessage_User_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReservationTraveler",
                columns: table => new
                {
                    ReservationTravelerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsChild = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationTraveler", x => x.ReservationTravelerId);
                    table.ForeignKey(
                        name: "FK_ReservationTraveler_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TripSeries",
                columns: table => new
                {
                    TripSeriesId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AgencyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripSeries", x => x.TripSeriesId);
                    table.ForeignKey(
                        name: "FK_TripSeries_Agency_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agency",
                        principalColumn: "AgencyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "UseCase",
                columns: new[] { "UseCaseId", "UseCaseDescription", "UseCaseName" },
                values: new object[,]
                {
                    { 54, "Account details of the signed-in user", "Get my profile" },
                    { 55, "Change own name, phone and address", "Update my profile" },
                    { 56, "Change own password", "Change password" },
                    { 57, "Correct the names of travelers on a reservation", "Update travelers" },
                    { 58, "Passenger list of a departure", "Get trip manifest" },
                    { 59, "Agency records a payment or refund", "Record payment" },
                    { 60, "Messages between the customer and the agency", "Get reservation messages" },
                    { 61, "Write to the customer or the agency about a reservation", "Send reservation message" },
                    { 62, "Other departures and similar trips", "Get similar trips" },
                    { 63, "Hide or show a destination", "Set destination active" },
                    { 64, "Hide or show a hotel", "Set hotel active" },
                    { 65, "Delete uploaded images that nothing uses", "Clean up images" },
                    { 66, "Agency adds another date of a trip", "Add departure" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trip_TripSeriesId",
                table: "Trip",
                column: "TripSeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_RecordedByUserId",
                table: "Payment",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_ReservationId",
                table: "Payment",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationMessage_ReservationId_SentAt",
                table: "ReservationMessage",
                columns: new[] { "ReservationId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationMessage_SenderUserId",
                table: "ReservationMessage",
                column: "SenderUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationTraveler_ReservationId",
                table: "ReservationTraveler",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_TripSeries_AgencyId",
                table: "TripSeries",
                column: "AgencyId");

            // Every existing trip becomes the only departure of its own series (same ID as the trip).
            migrationBuilder.Sql(@"
SET IDENTITY_INSERT [TripSeries] ON;
INSERT INTO [TripSeries] ([TripSeriesId], [AgencyId], [Name]) SELECT [TripId], [AgencyId], [TripName] FROM [Trip];
SET IDENTITY_INSERT [TripSeries] OFF;
UPDATE [Trip] SET [TripSeriesId] = [TripId];");

            // Existing accounts get the new use cases of their role (new accounts get them from the seed).
            migrationBuilder.Sql(@"
INSERT INTO [UserUseCase] ([UserId], [UseCaseId])
SELECT u.[UserId], uc.[UseCaseId]
FROM [User] u CROSS JOIN [UseCase] uc
WHERE NOT EXISTS (SELECT 1 FROM [UserUseCase] x WHERE x.[UserId] = u.[UserId] AND x.[UseCaseId] = uc.[UseCaseId])
  AND (
       (uc.[UseCaseId] IN (54, 55, 56, 62))
    OR (u.[Discriminator] = 'Regular user' AND uc.[UseCaseId] IN (57, 60, 61))
    OR (u.[Discriminator] = 'Agency employee' AND uc.[UseCaseId] IN (58, 59, 60, 61, 66))
    OR (u.[Discriminator] = 'Admin' AND uc.[UseCaseId] IN (63, 64, 65)))");

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_TripSeries_TripSeriesId",
                table: "Trip",
                column: "TripSeriesId",
                principalTable: "TripSeries",
                principalColumn: "TripSeriesId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [UserUseCase] WHERE [UseCaseId] BETWEEN 54 AND 66");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_TripSeries_TripSeriesId",
                table: "Trip");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "ReservationMessage");

            migrationBuilder.DropTable(
                name: "ReservationTraveler");

            migrationBuilder.DropTable(
                name: "TripSeries");

            migrationBuilder.DropIndex(
                name: "IX_Trip_TripSeriesId",
                table: "Trip");

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 66);

            migrationBuilder.DropColumn(
                name: "FailedLoginAttempts",
                table: "User");

            migrationBuilder.DropColumn(
                name: "LockedUntil",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TripSeriesId",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "CancellationFee",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Accommodation");
        }
    }
}
