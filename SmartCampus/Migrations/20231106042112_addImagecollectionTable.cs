using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartCampus.Migrations
{
    public partial class addImagecollectionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "AcademicResources",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImageCollections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ImagePath = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    AcademicResourceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageCollections_AcademicResources_AcademicResourceId",
                        column: x => x.AcademicResourceId,
                        principalTable: "AcademicResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageCollections_AcademicResourceId",
                table: "ImageCollections",
                column: "AcademicResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageCollections");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "AcademicResources");
        }
    }
}
