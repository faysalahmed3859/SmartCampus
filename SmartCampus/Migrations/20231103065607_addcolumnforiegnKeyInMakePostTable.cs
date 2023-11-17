using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartCampus.Migrations
{
    public partial class addcolumnforiegnKeyInMakePostTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MakePosts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MakePosts_UserId",
                table: "MakePosts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MakePosts_AspNetUsers_UserId",
                table: "MakePosts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MakePosts_AspNetUsers_UserId",
                table: "MakePosts");

            migrationBuilder.DropIndex(
                name: "IX_MakePosts_UserId",
                table: "MakePosts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MakePosts");
        }
    }
}
