using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig09 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a43f0148-2195-40a3-9cfe-1ccf088e4914");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bd68aa17-a33c-460c-88d2-6b3ce4d73286");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c8c75f32-a905-4513-b9e7-22a433b05983");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e2f418f0-3f7d-4896-ae38-26fb3b494293");

            migrationBuilder.AddColumn<bool>(
                name: "CashOnDelivery",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CashOnDelivery",
                table: "Orders");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "a43f0148-2195-40a3-9cfe-1ccf088e4914", "54d1fdc3-85bf-4d15-b674-87d0cd534fdd", "Client", "CLIENT" },
                    { "bd68aa17-a33c-460c-88d2-6b3ce4d73286", "b5b04546-ebf7-4256-9061-a5d452bb7ff9", "None", "NONE" },
                    { "c8c75f32-a905-4513-b9e7-22a433b05983", "701d37f4-8583-4868-b7fd-b98bc6f274cb", "Admin", "ADMIN" },
                    { "e2f418f0-3f7d-4896-ae38-26fb3b494293", "9a194994-d4b5-4c57-88bb-0e8abdcef5cb", "Moderator", "MODERATOR" }
                });
        }
    }
}
