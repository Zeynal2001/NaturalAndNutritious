using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig07 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "ShipName",
                table: "Orders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShippedDate",
                table: "Orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequiredDate",
                table: "Orders",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<Guid>(
                name: "ShipperId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("862B244B-D752-4B1B-90F5-D31D475D2F98"));

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "501f63d6-168b-4b0d-a06f-e02567829b4f", "dfc4e8be-5d02-4884-91af-0882a2919d5b", "Client", "CLIENT" },
                    { "d0033598-11c4-4b33-aeeb-cb44c6caa1c6", "d9e01f36-7307-4e3c-8fce-cdff58daf4c5", "Moderator", "MODERATOR" },
                    { "d2f4f817-45c6-40ed-851f-4858263a0a6a", "19479166-771a-48f2-840a-a76e7fc37efe", "None", "NONE" },
                    { "d92215c9-6a1b-4084-b59f-d869123467d3", "6d5686a6-dbd7-47d2-9431-f741d31637e0", "Admin", "ADMIN" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShipperId",
                table: "Orders",
                column: "ShipperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Shippers_ShipperId",
                table: "Orders",
                column: "ShipperId",
                principalTable: "Shippers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Shippers_ShipperId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShipperId",
                table: "Orders");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "501f63d6-168b-4b0d-a06f-e02567829b4f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d0033598-11c4-4b33-aeeb-cb44c6caa1c6");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2f4f817-45c6-40ed-851f-4858263a0a6a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d92215c9-6a1b-4084-b59f-d869123467d3");

            migrationBuilder.DropColumn(
                name: "ShipperId",
                table: "Orders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ShippedDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequiredDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
    }
}
