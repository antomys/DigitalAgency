using Microsoft.EntityFrameworkCore.Migrations;

namespace DigitalAgency.Dal.Migrations
{
    public partial class ActionType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Actions",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Actions");
        }
    }
}
