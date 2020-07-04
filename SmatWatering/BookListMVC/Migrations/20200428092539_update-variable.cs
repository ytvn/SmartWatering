using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartWatering.Migrations
{
    public partial class updatevariable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDay",
                table: "Variable");

            migrationBuilder.DropColumn(
                name: "UpdateDay",
                table: "Variable");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Variable",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDate",
                table: "Variable",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Variable");

            migrationBuilder.DropColumn(
                name: "UpdateDate",
                table: "Variable");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDay",
                table: "Variable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDay",
                table: "Variable",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
