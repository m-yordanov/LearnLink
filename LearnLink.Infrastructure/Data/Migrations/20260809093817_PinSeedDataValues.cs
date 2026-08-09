using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnLink.Migrations
{
    /// <inheritdoc />
    public partial class PinSeedDataValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e1f83c46-c1a5-4870-8447-b0b399035ac4", "AQAAAAEAACcQAAAAEHND0K+Y+rnCaFUzR+ussBps/28F7VBGNRvCXbOzv7mfCvU6622kNiFEdGGe1QPbTg==", "0db2227e-9f41-46a1-9df9-34e4290b622a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c2b15954-6a87-4207-8f3d-fb93ef5481f4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "007e0918-b993-45de-a254-8053bc10a141", "AQAAAAEAACcQAAAAEPlrrhtGUUqffS0i23TiGtrM75PsCR59OF+/L/DrKCk4ari7AwheSuXtHYXAtyc14w==", "590871c2-bbf8-4e02-9f4e-7b81b5b0e139" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c098-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f2b24258-c192-4ef8-bfc1-b6c15d2ccf32", "AQAAAAEAACcQAAAAEPuDCnn2OViyOCZCJDPhBC7UM/7unLPkmHsM3stDLuG8Z+O47DRS/tp7YAYBP76D/w==", "25e4616e-4818-4d50-b4d4-000bbf56b53e" });

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateAndTime",
                value: new DateTime(2026, 7, 11, 15, 7, 30, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateAndTime",
                value: new DateTime(2026, 7, 11, 15, 7, 30, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateAndTime",
                value: new DateTime(2026, 7, 11, 15, 7, 30, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateAndTime",
                value: new DateTime(2026, 7, 11, 15, 7, 30, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "729d91fe-9085-4220-a5aa-bda7c26636af", "AQAAAAEAACcQAAAAEGdC9vRkdBiD59ERltKAlSo8nBzoKUbHlu95yyAmuKmyTAVvoOlGkMcNiFmERYFhYg==", "00ddb872-156c-4168-a5f2-665d28b5771d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c2b15954-6a87-4207-8f3d-fb93ef5481f4",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7bda0a4d-7b8d-486d-9c8e-00df066db237", "AQAAAAEAACcQAAAAENAHPhZVMThUYWDHbQdJiCK83D+Ka6ojjmp0R7q9GsUWRAA0+3T5h0nrXb9tlNDWiA==", "b7fca371-b6e2-40b3-b171-e111e06cee19" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c098-4129-b3f3-b893d8395082",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fd93233f-f500-4cc0-8133-a03709081931", "AQAAAAEAACcQAAAAEEvSRP6bvCob1/pHFSqyhmmgp6I4/xq13qnr1azbeGCrmiDsdfHdaHjHDB2pFzK+Ag==", "ef85b0a2-ae01-4458-831e-b4565e76bc4b" });

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateAndTime",
                value: new DateTime(2026, 8, 6, 22, 37, 50, 86, DateTimeKind.Local).AddTicks(2260));

            migrationBuilder.UpdateData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateAndTime",
                value: new DateTime(2026, 8, 6, 22, 37, 50, 86, DateTimeKind.Local).AddTicks(2287));

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 1,
                column: "DateAndTime",
                value: new DateTime(2026, 8, 6, 22, 37, 50, 82, DateTimeKind.Local).AddTicks(4774));

            migrationBuilder.UpdateData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 2,
                column: "DateAndTime",
                value: new DateTime(2026, 8, 6, 22, 37, 50, 82, DateTimeKind.Local).AddTicks(4777));
        }
    }
}
