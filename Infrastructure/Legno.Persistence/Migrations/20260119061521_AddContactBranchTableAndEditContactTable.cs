using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Legno.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContactBranchTableAndEditContactTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContactBranchId",
                table: "Contacts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContactBranches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactBranches", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_ContactBranchId",
                table: "Contacts",
                column: "ContactBranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contacts_ContactBranches_ContactBranchId",
                table: "Contacts",
                column: "ContactBranchId",
                principalTable: "ContactBranches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contacts_ContactBranches_ContactBranchId",
                table: "Contacts");

            migrationBuilder.DropTable(
                name: "ContactBranches");

            migrationBuilder.DropIndex(
                name: "IX_Contacts_ContactBranchId",
                table: "Contacts");

            migrationBuilder.DropColumn(
                name: "ContactBranchId",
                table: "Contacts");
        }
    }
}
