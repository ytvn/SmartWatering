using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartWatering.Migrations
{
    public partial class editdevicetable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReadAPIKey",
                table: "Device",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WriteAPIKey",
                table: "Device",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReadAPIKey",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "WriteAPIKey",
                table: "Device");
        }
    }
}
