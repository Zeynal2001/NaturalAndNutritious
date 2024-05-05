using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4637d3ce-ae08-43e1-adfa-c7de62ec66cc", "f2c7e748-9d9f-4d70-8558-922718e0fe26", "Client", "CLIENT" },
                    { "92ebce47-9d5a-4a86-a01b-286b158cc743", "f81b9ae1-0868-4d89-b094-576c16164de7", "Moderator", "MODERATOR" },
                    { "a1ce9eaf-c0e6-4ff0-8a06-c50ccc687ec5", "2c74a585-dd72-4946-9448-49a3404370ee", "None", "NONE" },
                    { "cfaf5fa5-618a-491b-8b76-9e997664419a", "41c30e95-65d8-4439-86c5-7822ae6c268b", "Admin", "ADMIN" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4637d3ce-ae08-43e1-adfa-c7de62ec66cc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "92ebce47-9d5a-4a86-a01b-286b158cc743");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a1ce9eaf-c0e6-4ff0-8a06-c50ccc687ec5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cfaf5fa5-618a-491b-8b76-9e997664419a");
        }
    }
}
