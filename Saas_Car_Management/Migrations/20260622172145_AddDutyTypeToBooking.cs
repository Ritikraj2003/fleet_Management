using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Car_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddDutyTypeToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DutyTypeId",
                table: "Bookings",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_DutyTypeId",
                table: "Bookings",
                column: "DutyTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_DutyTypes_DutyTypeId",
                table: "Bookings",
                column: "DutyTypeId",
                principalTable: "DutyTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_DutyTypes_DutyTypeId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_DutyTypeId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DutyTypeId",
                table: "Bookings");
        }
    }
}
