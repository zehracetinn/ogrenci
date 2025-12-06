using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeOgrenciYonetim.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProjectApplications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "AdminUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProjectApplications");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "AdminUsers");
        }
    }
}
