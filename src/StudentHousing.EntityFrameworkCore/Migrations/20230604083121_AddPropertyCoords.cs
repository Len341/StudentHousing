using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHousing.Migrations
{
    public partial class AddPropertyCoords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyLocationCoords",
                table: "Property",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyLocationCoords",
                table: "Property");
        }
    }
}
