using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHousing.Migrations
{
    public partial class AddPropertyDistanceromUni : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DistanceFromUniversity",
                table: "Property",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceFromUniversity",
                table: "Property");
        }
    }
}
