using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1273a40a-6bb2-42c5-9a64-1603ed2f6b34");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "348b860a-8e7d-49c9-bf0d-bc7572c75241");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b1c6b5e2-3938-4af1-9aa0-aa92e7c9c39c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fe3047f8-f51d-4a9c-91bb-ab8cfc10bb71");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnswered",
                table: "ContactMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "087bfb4c-c06c-4ae5-8a06-ea7cd2d95401", "43fd9563-3097-4db2-8bbe-6a96cb0c4e10", "Client", "CLIENT" },
                    { "0c084ffb-2887-465f-b07f-f0e5537945d2", "354da61f-c5d1-4612-9ee2-dfff29eed8b8", "None", "NONE" },
                    { "42b3cd2b-58fa-4d1c-843c-8d77eec88b31", "62623797-59b5-4d08-8392-40c5e2820722", "Moderator", "MODERATOR" },
                    { "7bd4ba1e-612a-468e-91d7-6b6e9393a8c4", "23dd8a6c-f040-49b2-92b7-572a3f51a3ca", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "087bfb4c-c06c-4ae5-8a06-ea7cd2d95401");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0c084ffb-2887-465f-b07f-f0e5537945d2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "42b3cd2b-58fa-4d1c-843c-8d77eec88b31");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7bd4ba1e-612a-468e-91d7-6b6e9393a8c4");

            migrationBuilder.DropColumn(
                name: "IsAnswered",
                table: "ContactMessages");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1273a40a-6bb2-42c5-9a64-1603ed2f6b34", "e593a1ca-7fb0-4a74-8bae-7d445d33ca51", "Admin", "ADMIN" },
                    { "348b860a-8e7d-49c9-bf0d-bc7572c75241", "eb1bafcc-917a-45a4-bb3b-3107348aad2a", "Moderator", "MODERATOR" },
                    { "b1c6b5e2-3938-4af1-9aa0-aa92e7c9c39c", "01a8359f-6225-47fb-a7c5-7bfd3fa7233a", "Client", "CLIENT" },
                    { "fe3047f8-f51d-4a9c-91bb-ab8cfc10bb71", "d24693f5-5b7f-435d-b4fe-1caad508df53", "None", "NONE" }
                });
        }
    }
}
