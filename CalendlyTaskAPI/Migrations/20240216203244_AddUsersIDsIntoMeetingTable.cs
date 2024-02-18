using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalendlyTaskAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersIDsIntoMeetingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InitiatorUserId",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitiatorUserId",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Meeting");
        }
    }
}
