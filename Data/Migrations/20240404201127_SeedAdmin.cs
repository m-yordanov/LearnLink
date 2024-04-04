using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnLink.Migrations
{
    public partial class SeedAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "620b6167-2d93-45b4-aaa2-4796949e7580", "AQAAAAEAACcQAAAAEBGhPe5Fn0ueJsHsgazuIOrFsCSQSMDMmqO0ff/q7Uud3KwHCxWb00k+QiTZ3mk38A==", "bfb0e7c5-7d99-45bb-b9f6-b500a9092ee1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c098-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2dda76f9-2bd8-47fd-99cb-09a1b1f95759", "AQAAAAEAACcQAAAAEBil0GkfAyRlH9RhmC4kG/plJtGX3IK9N77Q+bUPzI/uYoeztq21aXWyUjE35j2Isw==", "97f18cb3-219f-4095-8610-f730ad78daec" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "c2b15954-6a87-4207-8f3d-fb93ef5481f4", 0, "b0bc2d19-5c4e-483a-b23e-40a7f140a160", "admin@mail.com", false, "The", "Admin", false, null, "admin@mail.com", "admin@mail.com", "AQAAAAEAACcQAAAAEON02ygRLsniRhaSCECaVClpFE+RoN6xcbu487nXU9tfUon+AR+ttW2v25MW6mduaw==", null, false, "6432b5bb-a294-4161-ba4d-c55c22657bcc", false, "admin@mail.com" });

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateAndTime",
                value: new DateTime(2024, 4, 4, 23, 11, 27, 668, DateTimeKind.Local).AddTicks(5219));

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateAndTime",
                value: new DateTime(2024, 4, 4, 23, 11, 27, 668, DateTimeKind.Local).AddTicks(5241));

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateAndTime",
                value: new DateTime(2024, 4, 4, 23, 11, 27, 665, DateTimeKind.Local).AddTicks(5083));

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateAndTime",
                value: new DateTime(2024, 4, 4, 23, 11, 27, 665, DateTimeKind.Local).AddTicks(5084));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c2b15954-6a87-4207-8f3d-fb93ef5481f4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "85a3338d-db9c-4e4f-a4d8-8dbb90505f41", "AQAAAAEAACcQAAAAEE2F/Csqrzb1vIN0K9ZcmOYxF3UfUrNp1l1T5ocnxe//j5uxKdcSN1vaDGjbQnbi7A==", "9ccc53b2-8acb-411b-a67b-7c363c110c50" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c098-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e31eb415-3262-41ed-9689-c6bf3e14c1a2", "AQAAAAEAACcQAAAAEO7La20l8REp2qNH1B82YWIgAiW79tL5KkJrYm+WkAwvY2jHymiVFpQvFwfEKwSmIA==", "8434d9c5-71f6-4c03-9185-5fd35af83834" });

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateAndTime",
                value: new DateTime(2024, 3, 29, 17, 19, 34, 926, DateTimeKind.Local).AddTicks(61));

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateAndTime",
                value: new DateTime(2024, 3, 29, 17, 19, 34, 926, DateTimeKind.Local).AddTicks(68));

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateAndTime",
                value: new DateTime(2024, 3, 29, 17, 19, 34, 923, DateTimeKind.Local).AddTicks(9778));

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateAndTime",
                value: new DateTime(2024, 3, 29, 17, 19, 34, 923, DateTimeKind.Local).AddTicks(9781));
        }
    }
}
