using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartWatering.Migrations
{
    public partial class reInit2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevicePin",
                columns: table => new
                {
                    PinId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PIN = table.Column<string>(nullable: true),
                    chipId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicePIn", x => x.PinId);
                });

            migrationBuilder.CreateTable(
                name: "Trigger",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariableId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    min = table.Column<float>(nullable: false),
                    max = table.Column<float>(nullable: false),
                    Interval = table.Column<int>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trigger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Variable",
                columns: table => new
                {
                    VariableId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VariableName = table.Column<string>(nullable: true),
                    PinId = table.Column<string>(nullable: true),
                    CreateDay = table.Column<DateTime>(nullable: false),
                    UpdateDay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variable", x => x.VariableId);
                });

            migrationBuilder.CreateTable(
                name: "VariableValue",
                columns: table => new
                {
                    VariableValueId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Value = table.Column<float>(nullable: false),
                    VariableId = table.Column<int>(nullable: false),
                    CreatedDay = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariableValue", x => x.VariableValueId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevicePin");

            migrationBuilder.DropTable(
                name: "Trigger");

            migrationBuilder.DropTable(
                name: "Variable");

            migrationBuilder.DropTable(
                name: "VariableValue");
        }
    }
}
