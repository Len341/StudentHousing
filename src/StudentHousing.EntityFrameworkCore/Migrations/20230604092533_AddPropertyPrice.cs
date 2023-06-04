using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHousing.Migrations
{
    public partial class AddPropertyPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthlyPrice",
                table: "Property",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthlyPrice",
                table: "Property");
        }
    }
}
