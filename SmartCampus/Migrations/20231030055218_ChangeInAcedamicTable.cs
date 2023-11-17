using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartCampus.Migrations
{
    public partial class ChangeInAcedamicTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUpload",
                table: "AcademicResources");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "AcademicResources");

            migrationBuilder.AddColumn<string>(
                name: "PdfPath",
                table: "AcademicResources",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfPath",
                table: "AcademicResources");

            migrationBuilder.AddColumn<string>(
                name: "FileUpload",
                table: "AcademicResources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "AcademicResources",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
