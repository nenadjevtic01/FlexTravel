using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlexTravel.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChildDiscountPercent",
                table: "Trip",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerNight",
                table: "ReservationRoom",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "AccommodationId",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ChildDiscountPercent",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Reservation",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Reservation",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerPerson",
                table: "Reservation",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Reservation",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StatusChangedAt",
                table: "Reservation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "UseCase",
                columns: new[] { "UseCaseId", "UseCaseDescription", "UseCaseName" },
                values: new object[,]
                {
                    { 7, "Search upcoming trips", "Search trips" },
                    { 8, "Trip details with itinerary, hotels and availability", "Get trip" },
                    { 9, "Price quote for a booking without saving it", "Calculate price" },
                    { 10, "Create a customer account", "Register user" },
                    { 11, "Book a trip", "Create reservation" },
                    { 12, "Reservations of the signed-in customer", "Get my reservations" },
                    { 13, "Customer cancels own reservation", "Cancel reservation" },
                    { 14, "Trips of the employee's agency with occupancy", "Get agency trips" },
                    { 15, "Agency publishes a new trip", "Create trip" },
                    { 16, "Agency withdraws a trip", "Deactivate trip" },
                    { 17, "Reservations for the agency's trips", "Get agency reservations" },
                    { 18, "Agency confirms or rejects a reservation", "Decide reservation" },
                    { 19, "Transports, destinations and hotels for the trip form", "Get trip form options" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_AccommodationId",
                table: "Reservation",
                column: "AccommodationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_Code",
                table: "Reservation",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Accommodation_AccommodationId",
                table: "Reservation",
                column: "AccommodationId",
                principalTable: "Accommodation",
                principalColumn: "AccommodationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Accommodation_AccommodationId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_AccommodationId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_Code",
                table: "Reservation");

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 19);

            migrationBuilder.DropColumn(
                name: "ChildDiscountPercent",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "PricePerNight",
                table: "ReservationRoom");

            migrationBuilder.DropColumn(
                name: "AccommodationId",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "ChildDiscountPercent",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "PricePerPerson",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "StatusChangedAt",
                table: "Reservation");
        }
    }
}
