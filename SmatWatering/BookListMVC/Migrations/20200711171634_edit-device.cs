using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartWatering.Migrations
{
    public partial class editdevice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Device",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Device");
        }
    }
}
