using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartCampus.Migrations
{
    public partial class addCulumnApproveInMakePostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "MakePosts",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "MakePosts");
        }
    }
}
