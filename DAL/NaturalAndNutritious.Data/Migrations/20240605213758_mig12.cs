using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5680f5a2-0706-425a-9f79-7bbd7317a419");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a5153e77-cf11-464e-8b0d-f433b518498f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f36112b5-767b-44ac-aa6d-3a981029a11c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f53e06f3-4a1d-4358-a060-819e196b9dac");

            migrationBuilder.AlterColumn<string>(
                name: "BlogPhotoUrl",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactMessages");

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

            migrationBuilder.AlterColumn<string>(
                name: "BlogPhotoUrl",
                table: "Blogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5680f5a2-0706-425a-9f79-7bbd7317a419", "bfb008d6-dced-4498-9827-9f0751bf4222", "Admin", "ADMIN" },
                    { "a5153e77-cf11-464e-8b0d-f433b518498f", "48825ae0-2132-4964-b3b3-3be915fa355f", "None", "NONE" },
                    { "f36112b5-767b-44ac-aa6d-3a981029a11c", "92ffdf5f-64b6-4b0a-8b0a-6b30101e9ba7", "Moderator", "MODERATOR" },
                    { "f53e06f3-4a1d-4358-a060-819e196b9dac", "f257fe6f-9b9d-4410-8456-3e5128bb1d71", "Client", "CLIENT" }
                });
        }
    }
}
