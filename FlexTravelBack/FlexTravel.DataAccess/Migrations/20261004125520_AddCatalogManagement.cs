using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlexTravel.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "UseCase",
                columns: new[] { "UseCaseId", "UseCaseDescription", "UseCaseName" },
                values: new object[,]
                {
                    { 46, "Add or edit a country with visa and safety info", "Save country" },
                    { 47, "Add or edit a destination with climate and images", "Save destination" },
                    { 48, "Destination with hotels, rooms, prices and sights for the catalog editor", "Get destination for edit" },
                    { 49, "Add or edit a hotel with room categories and prices", "Save hotel" },
                    { 50, "Add or edit a sight of a destination", "Save sight" },
                    { 51, "Remove a sight that no trip uses", "Delete sight" },
                    { 52, "Continents, hotel types, room types and sight categories", "Get catalog options" },
                    { 53, "Upload a destination image", "Upload image" }
                });

            // Existing admin accounts get the catalog use cases (new accounts get them from the seed).
            migrationBuilder.Sql(@"
INSERT INTO [UserUseCase] ([UserId], [UseCaseId])
SELECT u.[UserId], uc.[UseCaseId]
FROM [User] u CROSS JOIN [UseCase] uc
WHERE u.[Discriminator] = 'Admin' AND uc.[UseCaseId] BETWEEN 46 AND 53
  AND NOT EXISTS (SELECT 1 FROM [UserUseCase] x WHERE x.[UserId] = u.[UserId] AND x.[UseCaseId] = uc.[UseCaseId])");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [UserUseCase] WHERE [UseCaseId] BETWEEN 46 AND 53");

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 53);
        }
    }
}
