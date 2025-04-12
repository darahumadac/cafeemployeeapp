using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeEmployeeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEtagRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ETag",
                table: "Employees",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "ETag",
                table: "Cafes",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ETag",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ETag",
                table: "Cafes");
        }
    }
}
