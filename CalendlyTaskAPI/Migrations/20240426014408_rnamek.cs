using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendlyTaskAPI.Migrations
{
    /// <inheritdoc />
    public partial class rnamek : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InitiatorName",
                table: "UserNotifications",
                newName: "InitiatorUserId");

            migrationBuilder.AddColumn<string>(
                name: "InitiatorFullName",
                table: "UserNotifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InitiatorFullName",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitiatorFullName",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "InitiatorFullName",
                table: "Meeting");

            migrationBuilder.RenameColumn(
                name: "InitiatorUserId",
                table: "UserNotifications",
                newName: "InitiatorName");
        }
    }
}
