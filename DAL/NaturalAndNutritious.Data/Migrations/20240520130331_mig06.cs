using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig06 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<bool>(
                name: "Confirmed",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "11bf7493-b1a3-42fd-89e7-4c8b0a7b2702", "00afe3db-aaec-4b3a-9162-6b6852f76de7", "Admin", "ADMIN" },
                    { "305b2cf7-7699-464f-ae1d-086b353ff871", "fad89210-9a32-47fa-b64c-be16bad55b54", "Moderator", "MODERATOR" },
                    { "bc33b492-dc23-4fea-aa5b-01fee01f4232", "113b139e-eec0-40d5-82af-d14a1af8b64b", "None", "NONE" },
                    { "c238465d-9ed2-4ae7-8d6c-3e046e390235", "c3f46966-3e7d-40c9-b06e-000a8e01f83c", "Client", "CLIENT" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11bf7493-b1a3-42fd-89e7-4c8b0a7b2702");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "305b2cf7-7699-464f-ae1d-086b353ff871");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bc33b492-dc23-4fea-aa5b-01fee01f4232");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c238465d-9ed2-4ae7-8d6c-3e046e390235");

            migrationBuilder.DropColumn(
                name: "Confirmed",
                table: "Orders");

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
    }
}
