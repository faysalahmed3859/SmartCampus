using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartCampus.Migrations
{
    public partial class addgelleryAndPdfTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Galleries",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ImagePath = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    MakePostId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Galleries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Galleries_MakePosts_MakePostId",
                        column: x => x.MakePostId,
                        principalTable: "MakePosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PdfCollections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    PdfPath = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    AcademicResourceId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdfCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PdfCollections_AcademicResources_AcademicResourceId",
                        column: x => x.AcademicResourceId,
                        principalTable: "AcademicResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Galleries_MakePostId",
                table: "Galleries",
                column: "MakePostId");

            migrationBuilder.CreateIndex(
                name: "IX_PdfCollections_AcademicResourceId",
                table: "PdfCollections",
                column: "AcademicResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Galleries");

            migrationBuilder.DropTable(
                name: "PdfCollections");
        }
    }
}
