using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Legno.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditAnnouncementandArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubTitleEng",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubTitleRu",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleEng",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleRu",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubTitleEng",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubTitleRu",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleEng",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleRu",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTitleEng",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SubTitleRu",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "TitleEng",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "TitleRu",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SubTitleEng",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "SubTitleRu",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "TitleEng",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "TitleRu",
                table: "Announcements");
        }
    }
}
