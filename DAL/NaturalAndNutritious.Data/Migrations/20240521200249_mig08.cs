using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NaturalAndNutritious.Data.Migrations
{
    public partial class mig08 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "ShipVia",
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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "ShipVia",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
        }
    }
}
