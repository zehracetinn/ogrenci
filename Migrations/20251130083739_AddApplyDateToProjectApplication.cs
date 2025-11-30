using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjeOgrenciYonetim.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddApplyDateToProjectApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Students");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApplyDate",
                table: "ProjectApplications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyDate",
                table: "ProjectApplications");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
