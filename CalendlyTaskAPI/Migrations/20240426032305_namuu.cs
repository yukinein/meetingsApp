using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendlyTaskAPI.Migrations
{
    /// <inheritdoc />
    public partial class namuu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UserNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Meeting");
        }
    }
}
