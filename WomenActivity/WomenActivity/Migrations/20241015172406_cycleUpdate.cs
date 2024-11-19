using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WomenActivity.Migrations
{
    public partial class cycleUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mood",
                table: "CycleRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PainLevel",
                table: "CycleRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mood",
                table: "CycleRecords");

            migrationBuilder.DropColumn(
                name: "PainLevel",
                table: "CycleRecords");
        }
    }
}
