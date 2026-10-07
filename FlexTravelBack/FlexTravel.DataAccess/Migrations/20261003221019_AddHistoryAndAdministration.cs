using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlexTravel.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryAndAdministration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Trip",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "Trip",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationStatusChange",
                columns: table => new
                {
                    ReservationStatusChangeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationStatusChange", x => x.ReservationStatusChangeId);
                    table.ForeignKey(
                        name: "FK_ReservationStatusChange_Reservation_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservation",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReservationStatusChange_User_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "UseCase",
                columns: new[] { "UseCaseId", "UseCaseDescription", "UseCaseName" },
                values: new object[,]
                {
                    { 20, "Destinations that have upcoming trips", "Get trip destinations" },
                    { 21, "Reservation details with price breakdown and history", "Get reservation" },
                    { 22, "Agency edits its trip", "Update trip" },
                    { 23, "Trip data for the edit form", "Get trip for edit" },
                    { 24, "System overview for the admin", "Get admin stats" },
                    { 25, "Search user accounts", "Search users" },
                    { 26, "User account with permissions", "Get user" },
                    { 27, "Grant or revoke use cases of a user", "Update user use cases" },
                    { 28, "Block or unblock a user account", "Set user active" },
                    { 29, "Agencies with employees and results", "Get agencies" },
                    { 30, "Register a new partner agency", "Create agency" },
                    { 31, "Create an account for an agency employee", "Create agency employee" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trip_CreatedByUserId",
                table: "Trip",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_UpdatedByUserId",
                table: "Trip",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationStatusChange_ChangedByUserId",
                table: "ReservationStatusChange",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationStatusChange_ReservationId",
                table: "ReservationStatusChange",
                column: "ReservationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_User_CreatedByUserId",
                table: "Trip",
                column: "CreatedByUserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trip_User_UpdatedByUserId",
                table: "Trip",
                column: "UpdatedByUserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trip_User_CreatedByUserId",
                table: "Trip");

            migrationBuilder.DropForeignKey(
                name: "FK_Trip_User_UpdatedByUserId",
                table: "Trip");

            migrationBuilder.DropTable(
                name: "ReservationStatusChange");

            migrationBuilder.DropIndex(
                name: "IX_Trip_CreatedByUserId",
                table: "Trip");

            migrationBuilder.DropIndex(
                name: "IX_Trip_UpdatedByUserId",
                table: "Trip");

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 31);

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Trip");
        }
    }
}
