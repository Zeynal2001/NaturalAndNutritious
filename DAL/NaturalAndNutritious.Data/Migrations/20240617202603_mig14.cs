using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "Description",
                table: "Blogs");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "Blogs",
                newName: "Content");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalPhotoUrl1",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalPhotoUrl2",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1916cf7b-1d78-425e-95ac-f15d158d5509", "a8cbfb62-e996-4d6e-925c-78972f6dd5ab", "Admin", "ADMIN" },
                    { "863282c2-3e7f-4e80-a6b4-ad3b6c368098", "15909f5c-357a-493a-a1b0-69e598c63757", "Client", "CLIENT" },
                    { "b7431375-53db-48e4-af47-99424197eab6", "d811d100-33f5-4944-a990-f3f26f9ea4aa", "None", "NONE" },
                    { "cbd23540-ad5b-41cd-a819-946c8cdcb4c7", "9aa92c3f-f370-48ee-8630-afd14ff6bc29", "Moderator", "MODERATOR" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1916cf7b-1d78-425e-95ac-f15d158d5509");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "863282c2-3e7f-4e80-a6b4-ad3b6c368098");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7431375-53db-48e4-af47-99424197eab6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cbd23540-ad5b-41cd-a819-946c8cdcb4c7");

            migrationBuilder.DropColumn(
                name: "AdditionalPhotoUrl1",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "AdditionalPhotoUrl2",
                table: "Blogs");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Blogs",
                newName: "ShortDescription");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
    }
}
