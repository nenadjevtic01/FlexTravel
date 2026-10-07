using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlexTravel.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "User",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.InsertData(
                table: "UseCase",
                columns: new[] { "UseCaseId", "UseCaseDescription", "UseCaseName" },
                values: new object[,]
                {
                    { 78, "Upload or remove own profile photo", "Change my avatar" },
                    { 79, "Admin removes an unsuitable profile photo", "Remove user avatar" },
                    { 80, "Admin corrects a user's name, e-mail, phone and address", "Update user" }
                });

            // Existing accounts: everyone may change their own photo; admins also remove photos and edit users.
            migrationBuilder.Sql(@"
INSERT INTO [UserUseCase] ([UserId], [UseCaseId])
SELECT u.[UserId], uc.[UseCaseId]
FROM [User] u CROSS JOIN [UseCase] uc
WHERE NOT EXISTS (SELECT 1 FROM [UserUseCase] x WHERE x.[UserId] = u.[UserId] AND x.[UseCaseId] = uc.[UseCaseId])
  AND (uc.[UseCaseId] = 78 OR (u.[Discriminator] = 'Admin' AND uc.[UseCaseId] IN (79, 80)))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [UserUseCase] WHERE [UseCaseId] BETWEEN 78 AND 80");

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "UseCase",
                keyColumn: "UseCaseId",
                keyValue: 80);

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "User");
        }
    }
}
