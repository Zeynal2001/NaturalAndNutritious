using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ViewsCount",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4256d88e-c2bf-4cfd-9f71-e22e9591377f", "0cf54db0-56bb-49e0-a529-afe05e835120", "None", "NONE" },
                    { "6eda9e48-cd67-468c-a860-cb4494baa9dc", "91d0ec92-e860-4f5b-95dc-975ed463f0ec", "Admin", "ADMIN" },
                    { "8c835ae2-5db8-4da0-97de-92bbbc39a2e6", "22326463-3bff-4f16-b763-450dbd0838b8", "Client", "CLIENT" },
                    { "fdb12ca5-a56a-42ba-8246-001ff3be52d2", "953399fe-0870-4b07-a046-cf531f87f9e8", "Moderator", "MODERATOR" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "ViewsCount",
                table: "Products");

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
    }
}
