using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHousing.Migrations
{
    public partial class AddPropertyAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyAddress",
                table: "Property",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyAddress",
                table: "Property");
        }
    }
}
