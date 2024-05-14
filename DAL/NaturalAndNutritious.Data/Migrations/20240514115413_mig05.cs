using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig05 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "018c43d2-ce33-48ab-a654-838760560d9b");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49317f1c-ce8a-4335-a95a-50e942b151f6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a583f432-392d-4006-b3a2-2922177bec48");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b6eff095-3146-4ec4-8709-2acb8b378ac8");

            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlogPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ViewsCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "21ce7f75-08b0-4182-be0b-d8b956d802b8", "b52037b6-4951-4082-82e0-915d34bec172", "Client", "CLIENT" },
                    { "4ce0fa7d-5092-4857-9824-25de7abed668", "e5f05d9a-da68-4755-8281-56dcf33be0af", "Admin", "ADMIN" },
                    { "a8b4d469-9768-4955-b320-1bc1f0c24263", "dea7fffd-952c-47b0-a23d-69d66e885399", "Moderator", "MODERATOR" },
                    { "d04193e0-0bb2-4c18-8821-0a8af94484df", "9184678b-fbbc-4e50-949f-9b750aef3012", "None", "NONE" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "21ce7f75-08b0-4182-be0b-d8b956d802b8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4ce0fa7d-5092-4857-9824-25de7abed668");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a8b4d469-9768-4955-b320-1bc1f0c24263");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d04193e0-0bb2-4c18-8821-0a8af94484df");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "018c43d2-ce33-48ab-a654-838760560d9b", "f616d4be-1193-4eaf-83e8-f09457948259", "Moderator", "MODERATOR" },
                    { "49317f1c-ce8a-4335-a95a-50e942b151f6", "df8f4423-c5a1-48cb-923d-c8c26d43023c", "Admin", "ADMIN" },
                    { "a583f432-392d-4006-b3a2-2922177bec48", "0b898723-69a9-46b5-90ff-7c8159bd700f", "None", "NONE" },
                    { "b6eff095-3146-4ec4-8709-2acb8b378ac8", "61865e5b-e497-46f8-aea8-cbe0e78993b1", "Client", "CLIENT" }
                });
        }
    }
}
