using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartCampus.Migrations
{
    public partial class addclaInAcademiceTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AcademicResources",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AcademicResources_UserId",
                table: "AcademicResources",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicResources_AspNetUsers_UserId",
                table: "AcademicResources",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicResources_AspNetUsers_UserId",
                table: "AcademicResources");

            migrationBuilder.DropIndex(
                name: "IX_AcademicResources_UserId",
                table: "AcademicResources");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AcademicResources");
        }
    }
}
