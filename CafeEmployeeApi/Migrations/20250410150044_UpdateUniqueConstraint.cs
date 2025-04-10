using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeEmployeeApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_Name_Email_PhoneNumber_Gender",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Name_Email_PhoneNumber",
                table: "Employees",
                columns: new[] { "Name", "Email", "PhoneNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Employees_Name_Email_PhoneNumber",
                table: "Employees");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Name_Email_PhoneNumber_Gender",
                table: "Employees",
                columns: new[] { "Name", "Email", "PhoneNumber", "Gender" },
                unique: true);
        }
    }
}
