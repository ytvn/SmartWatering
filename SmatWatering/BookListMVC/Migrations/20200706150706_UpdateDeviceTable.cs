using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartWatering.Migrations
{
    public partial class UpdateDeviceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Device",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Device",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Device");
        }
    }
}
