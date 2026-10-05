using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthApi.Migrations
{
    /// <inheritdoc />
    public partial class updatetoentities09262026 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Technologies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 1L,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 2L,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 3L,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 4L,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 5L,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 6L,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                table: "Technologies",
                keyColumn: "TechnologyId",
                keyValue: 7L,
                column: "Description",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Technologies");
        }
    }
}
