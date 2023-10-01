using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eShop.Database.Migrations
{
    public partial class Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "1ecc2ade-13ba-4e2e-a2d6-fb082fe33774");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "07fa3feb-4361-4956-b958-50bef6ce894c", "AQAAAAEAACcQAAAAEC6H/v+UIFAgPBQHZjt05HoAdvlLUOENYE+tNCffIWNdgDQBk4xnbp97+AagTvVEXQ==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2023, 10, 1, 12, 53, 40, 240, DateTimeKind.Local).AddTicks(972));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AppRoles",
                keyColumn: "Id",
                keyValue: new Guid("8d04dce2-969a-435d-bba4-df3f325983dc"),
                column: "ConcurrencyStamp",
                value: "d642302d-4a8c-4053-9245-7c0d4053e63c");

            migrationBuilder.UpdateData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: new Guid("69bd714f-9576-45ba-b5b7-f00649be00de"),
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "5ecb0fe1-b2cb-454c-a92c-9c2cb17d3af2", "AQAAAAEAACcQAAAAEBRvTtsJUf7GbgJdLut/i6XYtenx23+gnYr4RsTEc76svWR81JRzkgNU1HXF5r++Lw==" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateCreated",
                value: new DateTime(2023, 8, 29, 14, 39, 59, 219, DateTimeKind.Local).AddTicks(1406));
        }
    }
}
