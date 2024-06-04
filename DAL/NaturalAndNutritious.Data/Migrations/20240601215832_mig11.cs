using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48f9a604-304a-4ab4-ab17-0c88fda7eccc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49a23069-06de-4ded-a4be-3dd5f3c82060");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "71f5b2c3-e74a-44d4-8eaf-c96eb8909ac5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b55bb423-d18d-4335-9ec0-45d772414dda");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MobileNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "MobileNumber",
                table: "Orders");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "48f9a604-304a-4ab4-ab17-0c88fda7eccc", "df10439d-6e78-4c66-acf6-f24232d887b0", "Admin", "ADMIN" },
                    { "49a23069-06de-4ded-a4be-3dd5f3c82060", "542b80c9-70e2-431d-8936-93e723d73019", "Client", "CLIENT" },
                    { "71f5b2c3-e74a-44d4-8eaf-c96eb8909ac5", "a077e86e-2259-4a84-975c-93b37cda5a9f", "Moderator", "MODERATOR" },
                    { "b55bb423-d18d-4335-9ec0-45d772414dda", "b2293a98-5583-4bb8-bfb5-b00f4f066005", "None", "NONE" }
                });
        }
    }
}
