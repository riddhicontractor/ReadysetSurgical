using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadySetSurgical.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_logs",
                table: "logs");

            migrationBuilder.RenameTable(
                name: "logs",
                newName: "errorLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_errorLogs",
                table: "errorLogs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_errorLogs",
                table: "errorLogs");

            migrationBuilder.RenameTable(
                name: "errorLogs",
                newName: "logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_logs",
                table: "logs",
                column: "Id");
        }
    }
}
