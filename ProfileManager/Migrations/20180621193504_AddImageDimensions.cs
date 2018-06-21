using Microsoft.EntityFrameworkCore.Migrations;

namespace ProfileManager.Migrations
{
    public partial class AddImageDimensions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PhotoHeight",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhotoWidth",
                table: "Employees",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoHeight",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PhotoWidth",
                table: "Employees");
        }
    }
}
