using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeEmployeeApi.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Employees",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Cafes",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Cafes");
        }
    }
}
