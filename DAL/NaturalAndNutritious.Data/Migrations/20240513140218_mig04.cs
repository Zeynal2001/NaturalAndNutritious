using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig04 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4256d88e-c2bf-4cfd-9f71-e22e9591377f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6eda9e48-cd67-468c-a860-cb4494baa9dc");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8c835ae2-5db8-4da0-97de-92bbbc39a2e6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fdb12ca5-a56a-42ba-8246-001ff3be52d2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "018c43d2-ce33-48ab-a654-838760560d9b", "f616d4be-1193-4eaf-83e8-f09457948259", "Moderator", "MODERATOR" },
                    { "49317f1c-ce8a-4335-a95a-50e942b151f6", "df8f4423-c5a1-48cb-923d-c8c26d43023c", "Admin", "ADMİN" },
                    { "a583f432-392d-4006-b3a2-2922177bec48", "0b898723-69a9-46b5-90ff-7c8159bd700f", "None", "NONE" },
                    { "b6eff095-3146-4ec4-8709-2acb8b378ac8", "61865e5b-e497-46f8-aea8-cbe0e78993b1", "Client", "CLİENT" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4256d88e-c2bf-4cfd-9f71-e22e9591377f", "0cf54db0-56bb-49e0-a529-afe05e835120", "None", "NONE" },
                    { "6eda9e48-cd67-468c-a860-cb4494baa9dc", "91d0ec92-e860-4f5b-95dc-975ed463f0ec", "Admin", "ADMİN" },
                    { "8c835ae2-5db8-4da0-97de-92bbbc39a2e6", "22326463-3bff-4f16-b763-450dbd0838b8", "Client", "CLİENT" },
                    { "fdb12ca5-a56a-42ba-8246-001ff3be52d2", "953399fe-0870-4b07-a046-cf531f87f9e8", "Moderator", "MODERATOR" }
                });
        }
    }
}
