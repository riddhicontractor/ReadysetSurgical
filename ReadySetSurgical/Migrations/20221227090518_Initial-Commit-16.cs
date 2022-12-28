using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReadySetSurgical.Migrations
{
    /// <inheritdoc />
    public partial class InitialCommit16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_invoiceDetails",
                table: "invoiceDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_errorLogs",
                table: "errorLogs");

            migrationBuilder.RenameTable(
                name: "invoiceDetails",
                newName: "details");

            migrationBuilder.RenameTable(
                name: "errorLogs",
                newName: "logs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_details",
                table: "details",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_logs",
                table: "logs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_logs",
                table: "logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_details",
                table: "details");

            migrationBuilder.RenameTable(
                name: "logs",
                newName: "errorLogs");

            migrationBuilder.RenameTable(
                name: "details",
                newName: "invoiceDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_errorLogs",
                table: "errorLogs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_invoiceDetails",
                table: "invoiceDetails",
                column: "Id");
        }
    }
}
