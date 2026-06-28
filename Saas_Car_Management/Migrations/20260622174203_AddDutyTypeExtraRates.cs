using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saas_Car_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddDutyTypeExtraRates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExtraHourRate",
                table: "DutyTypes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraKmRate",
                table: "DutyTypes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraHourRate",
                table: "DutyTypes");

            migrationBuilder.DropColumn(
                name: "ExtraKmRate",
                table: "DutyTypes");
        }
    }
}
