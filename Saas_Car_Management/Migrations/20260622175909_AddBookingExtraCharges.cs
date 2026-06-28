using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Car_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingExtraCharges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ActualDistance",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ActualHours",
                table: "Bookings",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraHourCharge",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraKmCharge",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualDistance",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ActualHours",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ExtraHourCharge",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ExtraKmCharge",
                table: "Bookings");
        }
    }
}
