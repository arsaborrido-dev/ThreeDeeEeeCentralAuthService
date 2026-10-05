using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class addedtechnologycategory09302026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 2L);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Technologies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<long>(
                name: "TechnologyCategoryId",
                table: "Technologies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "TechnologyCategories",
                columns: table => new
                {
                    TechnologyCategoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAtUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechnologyCategories", x => x.TechnologyCategoryId);
                });

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 1L,
                column: "TechnologyCategoryId",
                value: 3L);

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 3L,
                column: "TechnologyCategoryId",
                value: 3L);

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 4L,
                column: "TechnologyCategoryId",
                value: 3L);

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 5L,
                column: "TechnologyCategoryId",
                value: 3L);

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 6L,
                column: "TechnologyCategoryId",
                value: 3L);

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 7L,
                column: "TechnologyCategoryId",
                value: 3L);

            migrationBuilder.InsertData(
                table: "TechnologyCategories",
                columns: new[] { "TechnologyCategoryId", "DeletedAtUtc", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { 1L, null, "", false, "Frontend" },
                    { 2L, null, "", false, "Backend" },
                    { 3L, null, "", false, "Data" },
                    { 4L, null, "", false, "Security" },
                    { 5L, null, "", false, "Infrastructure" }
                });

            migrationBuilder.InsertData(
                table: "Technologies",
                columns: new[] { "TechnologyId", "DeletedAtUtc", "Description", "IsDeleted", "Name", "TechnologyCategoryId" },
                values: new object[,]
                {
                    { 8L, null, "", false, "ASP.NET Core MVC", 1L },
                    { 9L, null, "", false, "Razor", 1L },
                    { 10L, null, "", false, "HTML", 1L },
                    { 11L, null, "", false, "CSS", 1L },
                    { 12L, null, "", false, "JavaScript", 1L },
                    { 13L, null, "", false, "C#", 2L },
                    { 14L, null, "", false, "ASP.NET Core Web API", 2L },
                    { 15L, null, "", false, "Typed HttpClient", 2L },
                    { 16L, null, "", false, "PostgreSQL", 3L },
                    { 17L, null, "", false, "Entity Framework Core", 3L },
                    { 18L, null, "", false, "ASP.NET Core Identity", 4L },
                    { 19L, null, "", false, "JWT", 4L },
                    { 20L, null, "", false, "Cloudflare Turnstile", 4L },
                    { 21L, null, "", false, "Docker", 5L },
                    { 22L, null, "", false, "Docker Compose", 5L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Technologies_TechnologyCategoryId",
                table: "Technologies",
                column: "TechnologyCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TechnologyCategories_Name",
                table: "TechnologyCategories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Technologies_TechnologyCategories_TechnologyCategoryId",
                table: "Technologies",
                column: "TechnologyCategoryId",
                principalTable: "TechnologyCategories",
                principalColumn: "TechnologyCategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Technologies_TechnologyCategories_TechnologyCategoryId",
                table: "Technologies");

            migrationBuilder.DropTable(
                name: "TechnologyCategories");

            migrationBuilder.DropIndex(
                name: "IX_Technologies_TechnologyCategoryId",
                table: "Technologies");

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 10L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 11L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 12L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 13L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 14L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 15L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 16L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 17L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 18L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 19L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 20L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 21L);

            migrationBuilder.DeleteData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 22L);

            migrationBuilder.DropColumn(
                name: "TechnologyCategoryId",
                table: "Technologies");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Technologies",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.InsertData(
                table: "Technologies",
                columns: new[] { "TechnologyId", "DeletedAtUtc", "Description", "IsDeleted", "Name" },
                values: new object[] { 2L, null, "", false, "Docker" });
        }
    }
}
