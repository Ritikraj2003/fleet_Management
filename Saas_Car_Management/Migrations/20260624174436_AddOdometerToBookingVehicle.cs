using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Car_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddOdometerToBookingVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndOdometer",
                table: "BookingVehicles",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StartOdometer",
                table: "BookingVehicles",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndOdometer",
                table: "BookingVehicles");

            migrationBuilder.DropColumn(
                name: "StartOdometer",
                table: "BookingVehicles");
        }
    }
}
