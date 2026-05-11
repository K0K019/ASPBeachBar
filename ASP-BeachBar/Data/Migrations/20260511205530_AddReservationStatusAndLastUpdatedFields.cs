using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP_BeachBar.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationStatusAndLastUpdatedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "Events",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedOn",
                table: "Drinks",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "LastUpdatedOn",
                table: "Drinks");
        }
    }
}
