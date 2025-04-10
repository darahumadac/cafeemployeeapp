using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeEmployeeApi.Migrations
{
    /// <inheritdoc />
    public partial class AlterEmployeeCafeIdFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Cafes_CafeId",
                table: "Employees");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Cafes_CafeId",
                table: "Employees",
                column: "CafeId",
                principalTable: "Cafes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
               name: "FK_Employees_Cafes_CafeId",
               table: "Employees");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Cafes_CafeId",
                table: "Employees",
                column: "CafeId",
                principalTable: "Cafes",
                principalColumn: "Id");
        }
    }
}
