using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7390c68b-21d2-4f3e-be96-41d257a7e842");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c1826e32-bbc8-4c64-985c-dee3f00c7fc3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "caba5ff2-860c-44e5-952e-7d5fbc3a8fcf");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "df1a16b5-6e38-4445-8de6-6b2f55523bbb");

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscribers");

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

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "Orders");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7390c68b-21d2-4f3e-be96-41d257a7e842", "dcc5397f-caf6-4c51-89d3-a76e7befe734", "Moderator", "MODERATOR" },
                    { "c1826e32-bbc8-4c64-985c-dee3f00c7fc3", "827bb798-1947-4457-a1b8-2730bbbb0851", "Client", "CLIENT" },
                    { "caba5ff2-860c-44e5-952e-7d5fbc3a8fcf", "0607a8d2-2e6f-4230-bd4f-002d14487f66", "Admin", "ADMIN" },
                    { "df1a16b5-6e38-4445-8de6-6b2f55523bbb", "1ac5a033-d43a-4f7f-82f2-1e04c5babc83", "None", "NONE" }
                });
        }
    }
}
