using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Legno.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditAnnouncementandArticles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuthorNameEng",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorNameRu",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorNameEng",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AuthorNameRu",
                table: "Announcements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorNameEng",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AuthorNameRu",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AuthorNameEng",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "AuthorNameRu",
                table: "Announcements");
        }
    }
}
