using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Car_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddFareAndLocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BaseRate",
                table: "Cars",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "BaseRate",
                table: "BookingVehicles",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Distance",
                table: "BookingVehicles",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Fare",
                table: "BookingVehicles",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Hours",
                table: "BookingVehicles",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "BookingVehicles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RateType",
                table: "BookingVehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DropLocation",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PickupLocation",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseRate",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "BaseRate",
                table: "BookingVehicles");

            migrationBuilder.DropColumn(
                name: "Distance",
                table: "BookingVehicles");

            migrationBuilder.DropColumn(
                name: "Fare",
                table: "BookingVehicles");

            migrationBuilder.DropColumn(
                name: "Hours",
                table: "BookingVehicles");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "BookingVehicles");

            migrationBuilder.DropColumn(
                name: "RateType",
                table: "BookingVehicles");

            migrationBuilder.DropColumn(
                name: "DropLocation",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PickupLocation",
                table: "Bookings");
        }
    }
}
