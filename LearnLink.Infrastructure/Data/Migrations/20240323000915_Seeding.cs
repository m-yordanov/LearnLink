using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnLink.Migrations
{
    public partial class Seeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Teachers_TeacherId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_IdentityUserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_AspNetUsers_IdentityUserId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_IdentityUserId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_IdentityUserId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Students");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Teachers",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d", 0, "ba30fbd4-b8cf-442d-969d-264a338a76a2", "teacher@mail.com", false, false, null, "teacher@mail.com", "teacher@mail.com", "AQAAAAEAACcQAAAAEEPyrKoQwdo6nk5xsnx0x6r7asU85GtgvduJONZoDgDnjuDccxa8KVk4RF1AU2bmXA==", null, false, "b631a280-1c76-4054-a23a-0b89916b4a21", false, "teacher@mail.com" },
                    { "dea12856-c098-4129-b3f3-b893d8395082", 0, "578b6a1f-89ef-47c6-8184-6af6a818d9d4", "student@mail.com", false, false, null, "student@mail.com", "student@mail.com", "AQAAAAEAACcQAAAAEAb1vP18CsBHnW8HkJxjle7DEYQruvjoJnZH6IemLHYg+14XL0oEe0wK6cjCP4vZXg==", null, false, "3da892cb-5a7d-42ce-a90b-4d2da544d46e", false, "student@mail.com" }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "History" },
                    { 2, "Geography" }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "UserId" },
                values: new object[] { 1, "student@mail.com", "Ivan", "Petrov", "dea12856-c098-4129-b3f3-b893d8395082" });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "UserId" },
                values: new object[] { 1, "teacher@mail.com", "Viktor", "Georgiev", "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d" });

            migrationBuilder.InsertData(
                table: "Attendances",
                columns: new[] { "Id", "DateAndTime", "Status", "StudentId", "SubjectId", "TeacherId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 3, 23, 2, 9, 14, 798, DateTimeKind.Local).AddTicks(484), 0, 1, 1, 1 },
                    { 2, new DateTime(2024, 3, 23, 2, 9, 14, 798, DateTimeKind.Local).AddTicks(502), 2, 1, 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Grades",
                columns: new[] { "Id", "DateAndTime", "StudentId", "SubjectId", "TeacherId", "Value" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 3, 23, 2, 9, 14, 795, DateTimeKind.Local).AddTicks(7891), 1, 1, 1, 5.50m },
                    { 2, new DateTime(2024, 3, 23, 2, 9, 14, 795, DateTimeKind.Local).AddTicks(7894), 1, 2, 1, 5.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_UserId",
                table: "Students",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Teachers_TeacherId",
                table: "Attendances",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_AspNetUsers_UserId",
                table: "Teachers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_Teachers_TeacherId",
                table: "Attendances");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_AspNetUsers_UserId",
                table: "Students");

            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_AspNetUsers_UserId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_UserId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Students_UserId",
                table: "Students");

            migrationBuilder.DeleteData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Attendances",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Grades",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Teachers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6d5800ce-d726-4fc8-83d9-d6b3ac1f592d");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "dea12856-c098-4129-b3f3-b893d8395082");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Teachers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Teachers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_IdentityUserId",
                table: "Teachers",
                column: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_IdentityUserId",
                table: "Students",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_Teachers_TeacherId",
                table: "Attendances",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_AspNetUsers_IdentityUserId",
                table: "Students",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_AspNetUsers_IdentityUserId",
                table: "Teachers",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
