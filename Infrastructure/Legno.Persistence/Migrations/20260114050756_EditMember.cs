using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Legno.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisplayOrderId",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitleEng",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitleRu",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrderId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "JobTitleEng",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "JobTitleRu",
                table: "Members");
        }
    }
}
